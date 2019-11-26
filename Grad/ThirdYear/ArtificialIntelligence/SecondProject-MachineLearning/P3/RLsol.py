#Group 17: Francisco Barros - 85069, Rafael Ribeiro - 84758

# -*- coding: utf-8 -*-
"""
Created on Mon Oct 16 20:31:54 2017

@author: mlopes
"""
import numpy as np

def Q2pol(Q, eta=5):
	nS = len(Q)
	nA = len(Q[0])
	pol = np.zeros((nS, nA))
	for i, line in enumerate(Q):
		pol[i][np.argmax(line)] = 1
	return pol

class myRL:
	def __init__(self, nS, nA, gamma):
		self.nS = nS
		self.nA = nA
		self.gamma = gamma
		self.Q = np.zeros((nS,nA))
		
	def traces2Q(self, trace, alphaValue = 0.13):
		self.Q = np.zeros((self.nS, self.nA))
		for _ in range(100):
			for line in trace:
				originState, action, nextState, reward = (line).astype(int)
				currentQ = self.Q[originState][action]
				nextStateMax = np.amax(self.Q[nextState])
				newQ = currentQ + alphaValue * (reward + self.gamma * nextStateMax - currentQ)
				self.Q[originState][action] = newQ
		return self.Q

	def calculateRewardMatrix(self, trace):
		R = []
		for i in range(self.nS):
			line = []
			for j in range(self.nA):
				line.append([])
			R.append(line)
		
		for line in trace:
			originState, action, _, reward = (line).astype(int)
			currentR = R[originState][action]
			if reward not in currentR:
				currentR.append(reward)
				R[originState][action] = currentR

		#Prints the reward matrix.
		'''	
		for i in range(self.nS):
			line = R[i]
			print("\tState {}    ".format(i), end='')
			for j in range(len(line)):
				print(line[j], "    ", end='')
			print("")
		print("\n\n")
		'''
		return R

	def mapEnvironment(self, trace):
		E = []
		for i in range(self.nS):
			line = []
			for j in range(self.nA):
				line.append([])
			E.append(line)
		
		for line in trace:
			originState, action, nextState, _ = (line).astype(int)
			connected = E[originState][action]
			if nextState not in connected:
				connected.append(nextState)
				E[originState][action] = connected
		
		#Prints the states matrix.
		'''
		for i in range(self.nS):
			line = E[i]
			print("\tState {}    ".format(i), end='')
			for j in range(len(line)):
				print(line[j], "    ", end='')
			print("")
		print("\n\n")
		'''
		return E
		