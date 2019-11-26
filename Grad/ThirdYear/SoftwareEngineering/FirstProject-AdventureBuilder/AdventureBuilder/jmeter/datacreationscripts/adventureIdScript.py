
for i in range (0, 100):
	if i < 10:
		brokerCode = "B10{}".format(i)
		clientNif = "21111100{}".format(i)
		adventureCode = "B10{}{}".format(i, 1)

	elif i >= 10 < 100:
		brokerCode = "B1{}".format(i)
		clientNif = "2111110{}".format(i)
		adventureCode = "B1{}{}".format(i, 1)

	line1 = "{},{},{}".format(brokerCode, clientNif, adventureCode)
	print(line1)
