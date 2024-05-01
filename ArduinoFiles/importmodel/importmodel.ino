#include <ArduinoBLE.h>
//BLECharacteristic right("e", BLERead | BLENotify, 20);




/*
  IMU Classifier
  This example uses the on-board IMU to start reading acceleration and gyroscope
  data from on-board IMU, once enough samples are read, it then uses a
  TensorFlow Lite (Micro) model to try to classify the movement as a known gesture.
  Note: The direct use of C/C++ pointers, namespaces, and dynamic memory is generally
        discouraged in Arduino examples, and in the future the TensorFlowLite library
        might change to make the sketch simpler.
  The circuit:
  - Arduino Nano 33 BLE or Arduino Nano 33 BLE Sense board.
  Created by Don Coleman, Sandeep Mistry
  Modified by Dominic Pajak, Sandeep Mistry
  This example code is in the public domain.
*/

#include <Arduino_BMI270_BMM150.h>

#include <TensorFlowLite.h>
#include <tensorflow/lite/micro/all_ops_resolver.h>
#include <tensorflow/lite/micro/micro_error_reporter.h>
#include <tensorflow/lite/micro/micro_interpreter.h>
#include <tensorflow/lite/schema/schema_generated.h>
#include <tensorflow/lite/version.h>

#include "model.h"

const float accelerationThreshold = 2; // threshold of significant in G's
const int numSamples = 40;

int samplesRead = numSamples;

//global variables used for TensorFlow Lite (Micro)
tflite::MicroErrorReporter tflErrorReporter;

// pull in all the TFLM ops, you can remove this line and
// only pull in the TFLM ops you need, if would like to reduce
// the compiled size of the sketch.
tflite::AllOpsResolver tflOpsResolver;

const tflite::Model* tflModel = nullptr;
tflite::MicroInterpreter* tflInterpreter = nullptr;
TfLiteTensor* tflInputTensor = nullptr;
TfLiteTensor* tflOutputTensor = nullptr;

// Create a static memory buffer for TFLM, the size may need to
// be adjusted based on the model you are using
constexpr int tensorArenaSize = 8 * 1024;
byte buff[tensorArenaSize];
byte tensorArena[tensorArenaSize] __attribute__((aligned(16)));
byte buff2[tensorArenaSize] __attribute__((aligned(16)));

//array to map gesture index to a name
const char* GESTURES[] = {
  "left",
  "right",
  "straight",
  "up"
};

#define NUM_GESTURES (sizeof(GESTURES) / sizeof(GESTURES[0]))

BLEService QuaternionService("F");
BLECharacteristic left("1", BLERead | BLENotify, 20);
BLECharacteristic right("2", BLERead | BLENotify, 20);
BLECharacteristic straight("3", BLERead | BLENotify, 20);
BLECharacteristic up("4", BLERead | BLENotify, 20);
BLECharacteristic accel("5", BLERead | BLENotify, 20);



void setup() {
  Serial.begin(9600);

  

  // initialize the IMU
  if (!IMU.begin()) {
    Serial.println("Failed to initialize IMU!");
    while (1);
  }

  // print out the samples rates of the IMUs
  Serial.print("Accelerometer sample rate = ");
  Serial.print(IMU.accelerationSampleRate());
  Serial.println(" Hz");
  Serial.print("Gyroscope sample rate = ");
  Serial.print(IMU.gyroscopeSampleRate());
  Serial.println(" Hz");

  Serial.println();

  // get the TFL representation of the model byte array
  tflModel = tflite::GetModel(model);
  if (tflModel->version() != TFLITE_SCHEMA_VERSION) {
    Serial.println("Model schema mismatch!");
    while (1);
  }

  // Create an interpreter to run the model
  tflInterpreter = new tflite::MicroInterpreter(tflModel, tflOpsResolver, tensorArena, tensorArenaSize, &tflErrorReporter);

  // // Allocate memory for the model's input and output tensors
  tflInterpreter->AllocateTensors();

  // Get pointers for the model's input and output tensors
  tflInputTensor = tflInterpreter->input(0);
  tflOutputTensor = tflInterpreter->output(0);
  
      if (!BLE.begin()) {
        Serial.println("starting BLE failed!");

        while (1);
      }
    
    BLE.setLocalName("Controller");
    // BLE.setDeviceName("KONTROLLER");
    BLE.setAdvertisedService(QuaternionService); // add the service UUID
    QuaternionService.addCharacteristic(left);
    QuaternionService.addCharacteristic(right);
    QuaternionService.addCharacteristic(straight);
    QuaternionService.addCharacteristic(up);
    QuaternionService.addCharacteristic(accel);




    //QuaternionService.addCharacteristic(right);
    left.writeValue((byte)1);
    right.writeValue((byte)1);
    straight.writeValue((byte)1);
    up.writeValue((byte)1);
    accel.writeValue((byte)1);

    //right.writeValue((byte)2);
    BLE.addService(QuaternionService); 
    BLE.advertise();
}

bool flag = false;
String address;
float accelMax = 0;

void loop() {
  BLEDevice central = BLE.central();

  if(central & !flag)
  {
    Serial.print("Connected to :");
    Serial.println(central.address());
    address = central.address();
    flag = true;
  }
  if(!central & flag)
  {
    Serial.print("Disconnected from:");
    Serial.println(address);
    flag = false;
  }
  float aX, aY, aZ, gX, gY, gZ;

  // wait for significant motion
  if (samplesRead == numSamples) {
    if (IMU.accelerationAvailable()) {
      // read the acceleration data
      IMU.readAcceleration(aX, aY, aZ);

      // sum up the absolutes
      float aSum = fabs(aX) + fabs(aY) + fabs(aZ);

      // check if it's above the threshold
      if (aSum >= accelerationThreshold) {
        // reset the sample read count
        samplesRead = 0;
      }
    }
  }

  // check if the all the required samples have been read since
  // the last time the significant motion was detected
  if (samplesRead < numSamples) {
    Serial.print("samplesread: ");
    Serial.println(samplesRead);
    Serial.print("numsamples: ");
    Serial.println(numSamples);
    // check if new acceleration AND gyroscope data is available
    if (IMU.accelerationAvailable() && IMU.gyroscopeAvailable()) {
      // read the acceleration and gyroscope data
      IMU.readAcceleration(aX, aY, aZ);
      IMU.readGyroscope(gX, gY, gZ);
      float aSum = fabs(aX) + fabs(aY) + fabs(aZ);
      if (aSum>accelMax) accelMax = aSum;

      // normalize the IMU data between 0 to 1 and store in the model's
      // input tensor
      tflInputTensor->data.f[samplesRead * 6 + 0] = (aX + 4.0) / 8.0;
      tflInputTensor->data.f[samplesRead * 6 + 1] = (aY + 4.0) / 8.0;
      tflInputTensor->data.f[samplesRead * 6 + 2] = (aZ + 4.0) / 8.0;
      tflInputTensor->data.f[samplesRead * 6 + 3] = (gX + 2000.0) / 4000.0;
      tflInputTensor->data.f[samplesRead * 6 + 4] = (gY + 2000.0) / 4000.0;
      tflInputTensor->data.f[samplesRead * 6 + 5] = (gZ + 2000.0) / 4000.0;

      samplesRead++;
      Serial.print("sample is: ");
      Serial.println(samplesRead);
      if (samplesRead == numSamples) {
        //Run inferencing
        TfLiteStatus invokeStatus = tflInterpreter->Invoke();
        if (invokeStatus != kTfLiteOk) {
          Serial.println("Invoke failed!");
          while (1);
          return;
        }

        //Loop through the output tensor values from the model
        for (int i = 0; i < NUM_GESTURES; i++) {
          Serial.print(GESTURES[i]);
          Serial.print(": ");
          Serial.println(tflOutputTensor->data.f[i], 6);
          byte* buff = (byte*)&(tflOutputTensor->data.f[i]);
          if(central)
          {
            if ( i==0)
              left.writeValue(buff, sizeof(buff));
            else if (i==1)
              right.writeValue(buff, sizeof(buff));
            else if (i==2)
              straight.writeValue(buff, sizeof(buff));
            else if (i==3)
              up.writeValue(buff, sizeof(buff));
          }
          
        }
        byte* accelbuff = (byte*)&accelMax;
        accel.writeValue(accelbuff,sizeof(accelbuff));
        Serial.print(accelMax);
        accelMax = 0;
        Serial.println();
      }
    }
  }
}
