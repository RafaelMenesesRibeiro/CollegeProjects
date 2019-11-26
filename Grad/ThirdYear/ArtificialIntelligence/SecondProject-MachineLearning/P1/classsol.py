#Group 17: Francisco Barros - 85069, Rafael Ribeiro - 84758

import numpy as np
from sklearn import neighbors, datasets, tree, linear_model

from sklearn.externals import joblib
import timeit

from sklearn.model_selection import cross_val_score

def features(X):

	F = np.zeros((len(X),5))
	for x in range(0,len(X)):
		F[x,0] = len(X[x])
		F[x,1] = endsInVowel(X[x])
		F[x,2] = beginsInVowel(X[x])
		F[x,3] = numberVowels(X[x])
		F[x,4] = numberSpecialChars(X[x])

	return F

def mytraining(f,Y):
	clf = tree.DecisionTreeClassifier()
	clf = clf.fit(f, Y)
	return clf

def mytrainingaux(f,Y,par):
	return clf

def myprediction(f, clf):
	Ypred = clf.predict(f)
	return Ypred


def endsInVowel(word):
	code = ord(word[-1])
	if 224 <= code <= 255 or 97 <= code <= 111:
		return code
	return 0

def beginsInVowel(word):
	code = ord(word[0])
	if 224 <= code <= 255 or 97 <= code <= 111:
		return code
	return 0

def numberVowels(word):
	counter = 0
	for x in word:
		code = ord(x)
		if 224 <= code <= 255 or 97 <= code <= 111:
			counter += code
	return counter

def numberSpecialChars(word):
	counter = 0
	for x in word:
		code = ord(x)
		if 192 <= code <= 539:
			counter += code
	return counter
