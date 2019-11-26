
for i in range (0, 100):
	if i < 10:
		rentACarNif = "11111110{}".format(i)
		carPlate = "0{}-XX-0{}".format(i,i)
		nif = "21111110{}".format(i)
		iban = "RR123"


	elif i >= 10 and i < 100:
		rentACarNif = "1111111{}".format(i)
		carPlate = "{}-XX-{}".format(i,i)
		nif = "2111111{}".format(i)
		iban = "RR123"

	drivingLicense = "DV123"
	beginDate = "01/03/2018"
	endDate = "01/04/2018"
	line = "{},{},{},{},{},{},{}".format(rentACarNif, carPlate, nif, iban, drivingLicense, beginDate, endDate)
	print(line)
