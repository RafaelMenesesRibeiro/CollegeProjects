
for i in range (0, 100):
	if i < 10:
		brokerName = "B10{}".format(i)
	elif i >= 10 and i < 100:
		brokerName = "B1{}".format(i)
	
	bankCode = "BK12"
	line1 = "{},{}".format(bankCode, brokerName)
	print(line1)

for i in range (0, 100):
	if i < 10:
		brokerclientName = "B10{}C10{}".format(i, i)
	elif i >= 10 and i < 100:
		brokerclientName = "B1{}C1{}".format(i, i)
	
	bankCode = "BK12"
	line2 = "{},{}".format(bankCode, brokerclientName)
	print(line2)

#Activity Provider
bankCode = "BK12"
name = "AP1234"
line = "{},{}".format(bankCode, name)
print(line)

#Hotel
bankCode = "BK12"
name = "HT12435"
line = "{},{}".format(bankCode, name)
print(line)
