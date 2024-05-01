import serial
import os
f = open('straight.txt', 'w')
ser = serial.Serial("COM5", 9600)
while True:
     cc=str(ser.readline())
     f.write(cc[2:][:-5]+'\n')
     print(cc[2:][:-5])