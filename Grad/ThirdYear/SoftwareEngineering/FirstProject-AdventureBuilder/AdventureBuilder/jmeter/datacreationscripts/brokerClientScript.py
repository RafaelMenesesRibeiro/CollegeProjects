
for i in range (0, 100):
	if i < 10:
		brokerCode = "B10{}".format(i)
	elif i >= 10 and i < 100:
		brokerCode = "B1{}".format(i)

	clientIban = "BK12{}".format(i + 1 + 202 + 100)
	clientNif = "2111110{}".format(i)
	drivingLicense = "DL12345"
	age = 19
	line = "{},{},{},{},{}".format(brokerCode, clientIban, clientNif, drivingLicense, age)
	print(line)
