#Group 17: Francisco Barros - 85069, Rafael Ribeiro - 84758

import numpy as np
from sklearn import datasets, tree, linear_model
from sklearn.kernel_ridge import KernelRidge
from sklearn.svm import SVR
from sklearn.model_selection import GridSearchCV
from sklearn.model_selection import cross_val_score
import timeit


def mytraining(X,Y):
	#Kernel used in both Kernel Ridge Regression and Support Vector Regression.
	kernels = ['rbf']
	#Gammas used in both KRR and SVR.
	gammas = np.linspace(0, 1, num=100)
	#Alphas used only in KRR.
	alphas = np.linspace(0.0001, 1, num=100)
	#C parameters used only in SVR.
	Cs = np.linspace(1, 10000, num=10)

	#Change the value of useModel1 to False to use SVR. If True, the method used
	#will be KRR.
	useModel1 = False
	#Model 1: KRR.
	model = KernelRidge()
	#Parameters to test with the KRR model.
	params = dict(kernel=kernels, alpha=alphas, gamma=gammas)
	#If the variable useModel1 is False, uses model 2: SVR.
	if not useModel1:
		model = SVR()
		#Parameters to test with the SVR model.
		params = dict(kernel=kernels, C=Cs, gamma=gammas)

	#Uses a grid search and cross-validation (default value is 3 for KFold) to
	#obtain the best partameters for the chosen model for the dataset being used.
	grid = GridSearchCV(estimator=model, param_grid=params, scoring = 'neg_mean_squared_error')
	grid.fit(X, Y)
	#Gets the best kernel found.
	kernel = grid.best_estimator_.kernel
	#Gets the best gamma found.
	p1 = grid.best_estimator_.gamma
	if useModel1:
		#Gets the best alpha foundm if the model used was KRR.
		p2 = grid.best_estimator_.alpha
		#Creates a new model with the best found parameters.
		reg = KernelRidge(kernel = kernel, gamma = p1, alpha = p2)
		print("Best kernel: ", grid.best_estimator_.kernel, "\nBest gamma: ", grid.best_estimator_.gamma, "\nBest alpha: ", grid.best_estimator_.alpha)
	else:
		#Gets the best C found if the model used was SVR.
		p2 = grid.best_estimator_.C
		#Creates a new model with the best found parameters.
		reg = SVR(kernel = kernel, gamma = p1, C = p2)
		print("Best kernel: ", grid.best_estimator_.kernel, "\nBest gamma: ", grid.best_estimator_.gamma, "\nBest C: ", grid.best_estimator_.C)	
	#Fits the newly created model with the best found parameters. KRR if useModel1
	#is True. SVR is useModel1 is False.
	reg.fit(X, Y)
	#Returns the fitted model for prediction.
	return reg

def mytrainingaux(X,Y,par):
	reg.fit(X,Y)
	return reg

def myprediction(X,reg):
	Ypred = reg.predict(X)
	return Ypred
