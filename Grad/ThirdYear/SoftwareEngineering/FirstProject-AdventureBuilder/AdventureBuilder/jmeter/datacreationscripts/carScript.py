for i in range (0, 100):
	if i < 10:
		rentACarNif = "11111110{}".format(i)
		carPlate = "0{}-XX-0{}".format(i,i)
		kilometers = 10
		drivingLicense = "DV123"
		carPlate = "00-XX-00"
		price = 60.0

	elif i >= 10 and i < 100:
		rentACarNif = "1111111{}".format(i)
		carPlate = "{}-XX-{}".format(i,i)
		kilometers = 10
		price = 60.0

	line = "{},{},{},{},{}".format(rentACarNif, carPlate, kilometers, price, "CAR")
	print(line)