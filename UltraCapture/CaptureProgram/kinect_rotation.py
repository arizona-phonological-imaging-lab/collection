# 2/2015 Created by Sam Johnston

from __future__ import division
import os, sys, shutil
# import cv2
from os.path import join
from collections import defaultdict
from imaging import RichImage as RI
from imaging import Displayer
# import numpy as np
import wave as wav

# print dir(wav)

class KinectRotation():

	def __init__(self, argv):
		self.path = os.getcwd()
		self.MAXYAW = 5
		self.MAXROLL = 5
		self.MAXPITCH = 3

		self.vidpath = argv[1]
		self.coordspath = argv[2]
		self.framespath = argv[3]
		self.wavpath = argv[4]
		self.kinect_ref_file = argv[5] #this is the time stamp for the three ref coords for each pitch yaw roll
		self.dirpath = argv[6]
		# print "{0}\n{1}\n{2}".format(self.vidpath, self.coordspath, self.framespath)

		# self.vidpath = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/vidTimes.txt'
		# self.coordspath = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/coords.txt'
		# self.framespath = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/frames/'
		# self.wavpath = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/AllWorking1.wav'
		# self.kinect_ref_file = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/ref_coords.txt'
		# self.dirpath = 'C:/Users/apiladmin/Desktop/2015-05-01_AllWorking1/'



		f = open(self.kinect_ref_file,"r").readlines()
		line = f[0].split('\t')
		self.kinect_ref_coords = [line[0], line[1], line[2]] 

		# self.vidpath = 
		# self.coordspath = 
		self.rotated_framespath = join(self.dirpath,"adjusted_frames")
		if not os.path.isdir(self.rotated_framespath):
			os.makedirs(self.rotated_framespath)
		# self.wavpath =

	def main(self):
		self.read_vidTimes()
		self.get_framerate()
		self.read_coords()
		self.rotate()


	#finds the line that lists the start of the recording
	def read_vidTimes(self):	
		f = open(self.vidpath,'r').readlines()
		#obtains the beginning and end times of the recording
		start = f[1].split()
		end = f[2].split()
		self.start_string_time = start[3].strip()
		self.end_string_time = end[3].strip()
		self.startTime = self.get_subtime(self.start_string_time)
		self.endTime = self.get_subtime(self.end_string_time)

	def read_coords(self):
		#stores the kinect rotation values
		print "Storing Kinect Coordinate Data..."
		coords_time_dict = {}
		frame_time_dict = {}
		self.frame_coords_dict = {}

		f = open(self.coordspath,'r').readlines()

		i = 1; #current frame
		j = 0; #current line in coords

		while i <= len(self.frames):
			try:
				l = f[j]
			except IndexError:
				l = f[j-1]
			line = l.split()
			
			#passes kinect coords recorded prior to the start of the ultrasound recording
			if int(line[1]) < int(self.start_string_time):
				j+=1
				continue
			else:
				#compute the time of the frame and of the coords row, store them.
				if i in frame_time_dict:
					frame_time = frame_time_dict[i]
				else:
					frame_time_dict[i] = i * self.frame_rate
					frame_time = frame_time_dict[i]
				if j in coords_time_dict:
					coord_time = coords_time_dict[j]
				else:
					coords_time_dict[j] = self.get_subtime(line[1])
					coord_time = coords_time_dict[j]

				#compute the difference between the frame time and two different coords times; see which coords time is closer
				#if the new one is closer, move ahead to see if the next coords time is even closer. Otherwise assign the coords
				#of the old coords time.
				try:
					old_time_diff = abs(self.comparetime(frame_time,coords_time_dict[j-1]))
				except KeyError:
					#a sort-of hack to indicate that there is no old time diff to compare to; forces a fail at the next check 
					old_time_diff = 1000

				new_time_diff = abs(self.comparetime(frame_time,coord_time))

				if old_time_diff < new_time_diff:
					#obtain the degrees rotation
					self.get_coords(l)
					self.frame_coords_dict[i] = [round(self.pitch-float(self.kinect_ref_coords[0])), round(self.yaw-float(self.kinect_ref_coords[1])), round(self.roll-float(self.kinect_ref_coords[2]))]
					i+=1
				else:
					old_time_diff = new_time_diff
					j+=1


	def rotate(self):
		print "Applying Rotations..."
		#determine rotation for each frame
		self.rotation_log = open(join(self.dirpath,"rotation_log.txt"),"w")
		self.rotation_log.write("IMAGE\tROTATION\n")
		for image in self.frames:
			frame_num = int(image[-10:-4])
			pitch = self.frame_coords_dict[frame_num][0]
			yaw = self.frame_coords_dict[frame_num][1]
			roll = self.frame_coords_dict[frame_num][2] 

			if yaw > self.MAXYAW:
				shutil.copy2(join(self.framespath,image),join(self.rotated_framespath,image[:-4]+"_BAD.png"))
				self.log(image[:-4]+"_BAD.png","YAW > {0}".format(self.MAXYAW))
				continue
			if roll > self.MAXROLL:
				shutil.copy2(join(self.framespath,image),join(self.rotated_framespath,image[:-4]+"_BAD.png"))
				self.log(image[:-4]+"_BAD.png","ROLL > {0}".format(self.MAXROLL))
				continue
			if pitch > self.MAXPITCH:
				self.call_rotate(join(self.framespath,image),join(self.rotated_framespath,image[:-4]+"_ROTATED.png"),pitch)
				self.log(image[:-4]+"_ROTATED.png",pitch)


	def log(self,image,rotation):
		self.rotation_log.write("{0}\t{1}\n".format(image,rotation))



	def comparetime(self, firsttime, secondtime):
		#rather proud of this one - converts hour/minute/second/millisecond notation to second.millisecond notation, simultaneously subtracting the start time
		#so as to get a relative value to the start of the recording.
		# time_diff = (((((firsttime[0]-secondtime[0])*60)+(firsttime[1]-secondtime[1])*60)+(firsttime[2]-secondtime[2])*1000)+(firsttime[3]-secondtime[3])/1000)
		time2 = abs((((((((self.startTime[0] - secondtime[0])*60)+(self.startTime[1] - secondtime[1]))*60)+(self.startTime[2] - secondtime[2]))*1000)+(self.startTime[3] - secondtime[3]))/1000)
		time_diff = firsttime - time2

		return time_diff

	#extract the coordinate values from the current line
	def get_coords(self,line):
		idx = line.find("(")
		coords = line.strip()[idx:-1]
		pitch, yaw, roll = coords.split(",")
		#the degree sign composes two characters when indexing, hence using [:-2] to remove it from the end.
		self.pitch = float(pitch.split()[1][:-2]) #pitch ref coords
		self.yaw = float(yaw.split()[1][:-2]) #yaw ref coords
		# there is no degree symbol, so no need to [:-2] here
		self.roll = float(roll.split()[1]) #roll ref coords


	#extracts the time in hour, minute, second, millisecond of the recording
	def get_subtime(self,string_time):
		ms = string_time[-3:]
		sec = string_time[-5:-3]
		minit = string_time[-7:-5]
		hour = string_time[-9:-7]
		realTime = [int(hour),int(minit),int(sec),int(ms)]
		return realTime

	def get_framerate(self):
		self.frames = os.listdir(self.framespath)

		#code to obtain duration of wave file here:
		audio = wav.open(self.wavpath)
		sample_rate = audio.getframerate()
		sample_num = audio.getnframes()
		wave_duration = sample_num / sample_rate
		#computing video frame rate from the duration of the wave file
		self.frame_rate = wave_duration / len(self.frames)


	def call_rotate(self,frame_path,save_path,degrees):
		rich_img = RI(frame_path)#cv2.imread(frame_path))
		displayer = Displayer(rich_img)
		displayer.display_image(degrees, save_path)


if __name__ == "__main__":
	_reserved = KinectRotation(sys.argv)
	_reserved.main()