
for i in range (0, 100):
	if i < 10:
		name1 = "BBroker00{}".format(i)
		nif1 = "11111100{}".format(i)

		name2 = "SBroker00{}".format(i)
		nif2 = "11111200{}".format(i)

		name3 = "Client00{}".format(i)
		nif3 = "21111100{}".format(i)

	elif i >= 10 < 100:
		name1 = "BBroker0{}".format(i)
		nif1 = "1111110{}".format(i)

		name2 = "SBroker0{}".format(i)
		nif2 = "1111120{}".format(i)

		name3 = "Client0{}".format(i)
		nif3 = "2111110{}".format(i)

	street = "Street"
	typ1 = "BUYER"
	typ2 = "SELLER"

	line1 = "{},{},{},{}".format(name1, street, nif1, typ1)
	line2 = "{},{},{},{}".format(name2, street, nif2, typ2)
	line3 = "{},{},{},{}".format(name3, street, nif3, typ1)

	print(line1)
	print(line2)
	print(line3)


#Activity Provider
name = "AP1234"
street = "Street"
nif = "311111111"
typ = "SELLER"
line = "{},{},{},{}".format(name, street, nif, typ)
print(line)