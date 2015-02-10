func Foobar() {
	r := getRandom(1, 2, 3,
		5, 6, 7,
		8, 9, 0)

	if r > 10 {
		print("yes")
	} else {
		print("no")
	}
}