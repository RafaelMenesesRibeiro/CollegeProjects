import numpy as np
from sklearn import datasets, tree, linear_model
from sklearn.kernel_ridge import KernelRidge
from sklearn.model_selection import cross_val_score
from sklearn.externals import joblib
import timeit
import matplotlib.pyplot as plt
from matplotlib.colors import ListedColormap
from sklearn.utils.validation import column_or_1d

import regsol

tres = [.3, 800]    
for ii,test in enumerate(["regress.npy", "regress2.npy"]):
	print("Testing " + test)
	
	X,Y,Xp,Yp = np.load(test)
	Y = column_or_1d(Y)
	reg = regsol.mytraining(X,Y)
	
	Ypred = regsol.myprediction(Xp,reg)
	
	res = -cross_val_score( reg, X, Y, cv = 5, scoring = 'neg_mean_squared_error').mean()
	print(res)
	if -cross_val_score( reg, X, Y, cv = 5, scoring = 'neg_mean_squared_error').mean() < tres[ii]:
		print("Erro dentro dos limites de tolerância. OK\n")
	else:
		print("Erro acima dos limites de tolerância. FAILED\n")    
	plt.figure()
	plt.plot(Xp,Yp,'g.',label='datatesting')
	plt.plot(X,Y,'k+',label='datatrain')
	plt.plot(Xp,Ypred,'m',label='linregres1')
	plt.legend( loc = 1 )


