package input

import (
	"bufio"
	"os"
	"strings"
)

func ReadLines(file string) ([]string, error) {
	data, err := os.ReadFile(file)
	if err != nil {
		return nil, err
	}
	lines := strings.Split(string(data), "\n")
	return lines, nil
}

func FromStdin() ([]string, error) {
	var data []string
	scanner := bufio.NewScanner(os.Stdin)

	for scanner.Scan() {
		data = append(data, scanner.Text())
	}
	return data, scanner.Err()
}
