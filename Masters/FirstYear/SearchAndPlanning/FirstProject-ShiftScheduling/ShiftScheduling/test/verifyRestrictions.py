def isSolutionGood(shifts):
	for shift in shifts[2:]:
		tasks = shift.split("(")
		parsedTasks = []
		for task in tasks:
			task = task.replace("\n", "")
			task = task.replace(")", "")
			props = task.split(" ")
			parsedTasks.append([props[0], props[1], props[2], props[3]])
		
		if (len(parsedTasks) > 1):
			
			# Restriction - start to finnish in less than 60*8 minutes
			lastTask = parsedTasks[-1]
			firstTask = parsedTasks[0]
			endTime = int(lastTask[3])
			startTime = int(firstTask[2])
			if (endTime - startTime > 60*8):
				print("Violated Restriction 1: {}".format(parsedTasks))
				return 0

			# Restriction - if end and start are different, 40 minute interval
			for tn in range(1, len(parsedTasks)):
				endLocal = parsedTasks[tn-1][1]
				startLocal = parsedTasks[tn][0]
				if (endLocal != startLocal):
					startTime = int(parsedTasks[tn][2])
					endTime = int(parsedTasks[tn-1][3])
					if (startTime - endTime < 40):
						print(startTime, endTime)
						print("Violated Restriction 2: {}".format(parsedTasks))
						return 0

			# Restriction - if duration of this and following are longer than 60*4 minutes
			for tn in range(1, len(parsedTasks)):
				startTime = int(parsedTasks[tn-1][2])
				endTime = int(parsedTasks[tn][3])
				if (endTime - startTime > 60*4):
					endLocal = parsedTasks[tn-1][1]
					startLocal = parsedTasks[tn][0]
					if (endLocal != startLocal):
						sT = int(parsedTasks[tn][2])
						eT = int(parsedTasks[tn-1][3])
						if (endTime - startTime < 80):
							print("Violated Restriction 3: {}".format(parsedTasks))
							return 0
					else:
						sT = int(parsedTasks[tn][2])
						eT = int(parsedTasks[tn-1][3])
						if (endTime - startTime < 40):
							print("Violated Restriction 4: {}".format(parsedTasks))
							return 0
	return 1

import os

currentDict = os.getcwd()
folderPath = os.path.join(currentDict, "outs")

for file in os.listdir(folderPath):
	if file.endswith(".txt"):
		fileName = os.path.join(folderPath, file)

		f = open(fileName,"r")
		contents = f.read()
		shifts = contents.split("((")
		print(isSolutionGood(shifts))
		f.close()


