import math
import numpy as np
import matplotlib.pyplot as plt
from sklearn.externals import joblib
from RL import *
import RLsol


def testOnce(traj, alphaValue):
	Q = qlearn.traces2Q(traj, alphaValue)
	#print("Valores Q aprendidos")
	#print(qlearn.Q)
	#print("Diff = ", np.linalg.norm(Q - fmdp.Q))
	if np.linalg.norm(Q - fmdp.Q) < .3:
		print("Erro nos Q dentro dos limites de tolerância. OK\n")
	else:
		print("Erro nos Q acima dos limites de tolerância. FAILED\n")
	return Q

def findAlphas(traj):
	alphaValue = 0
	counter = 0
	validAlphas = []
	while True:
		Q = qlearn.traces2Q(traj, alphaValue)
		if np.linalg.norm(Q-fmdp.Q)<.3:
			validAlphas.append(alphaValue)
			counter += 1
			if counter >= 20:
				break
		alphaValue += 0.01
	
	return Q, validAlphas

def getTraj(fmdp, Q):
		J, trajlearn = fmdp.runPolicy(4, 5, RLsol.Q2pol(Q))
		if J > .7:
			print("Recompensa obtida dentro do previsto. OK\n")
		else:
			print("Recompensa obtida abaixo do previsto. FAILED\n")
		return J, trajlearn


def maximizeJ(fmdp):
	Q, validAlphas = findAlphas(traj)
	rews = []
	for alpha in validAlphas:
		Q = testOnce(traj, alpha)
		J = getTraj(fmdp, Q)
		rews.append(J)
	for i in range(len(validAlphas)):
		if rews[i] > 0.7:
			print("alpha = {}. 	Reward = {}".format(validAlphas[i], rews[i]))


def plotEnv(E):
	plt.figure()
	
	nS = len(E)
	nA = len(E[0])
	radius = 1
	angle = (360 / nS) *(3.14 / 180)
	style = [{'color':'green', 'linewidth':4}, {'color':'red', 'linewidth':2}]
	print(E)
	for i, state in enumerate(E):
		x = math.cos(angle * i) * radius
		y = math.sin(angle * i) * radius
		plt.plot(x, y, 'ro')
		plt.annotate('State{}'.format(i), xy=(x, y), xytext=(-20, 20), textcoords='offset points', bbox=dict(alpha=0.5))

		for j in range(nA):
			for nextState in E[i][j]:
				print("{} to {} with {}".format(i, nextState, j))
				p2 = [math.cos(angle * nextState) * radius, math.sin(angle * nextState) * radius]
				plt.plot([x, p2[0]], [y, p2[1]], color = style[j]['color'], linewidth = style[j]['linewidth'])

	plt.show()

for test in [('fmdp1.pkl','traj1.pkl'), ('fmdp2.pkl','traj2.pkl')]:
	print("Testing " + test[0])    
	# funcoes auxiliarres
	fmdp = joblib.load(test[0]) 
	# ficheiro com a trajectório de treino             
	traj = joblib.load(test[1]) 
	
	qlearn = RLsol.myRL(7, 2, 0.9)
	Q = testOnce(traj, 0.13)
	#Q, _ = findAlphas(traj)
	J, trajlearn = getTraj(fmdp, Q)

	#maximizeJ(fmdp)

	#R = qlearn.calculateRewardMatrix(traj)
	E = qlearn.mapEnvironment(traj)
	#plotEnv(E)
