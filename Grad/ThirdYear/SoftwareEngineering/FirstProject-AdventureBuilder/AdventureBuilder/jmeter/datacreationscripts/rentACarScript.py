for i in range (0, 100):
	if i < 10:
		rentACarNif = "11111110{}".format(i)

	elif i >= 10 and i < 100:
		rentACarNif = "1111111{}".format(i)

	rentACarIban = "RC{}".format(i+1+202)
	rentACarName = "Drive Happy"
	line = "{},{},{}".format(rentACarName, rentACarNif, rentACarIban)
	print(line)
