package utils

import (
	"io"
	"os"
)

func ReadLines() []string {
	data, err := io.ReadAll(os.Stdin)
	if err != nil {
		panic(err)
	}
	result := make([]string, 0)

	buf := ""
	for _, b := range data {
		if b == '\n' {
			result = append(result, buf)
			buf = ""
		} else {
			buf += string(b)
		}
	}

	if len(buf) > 0 {
		result = append(result, buf)
	}

	return result
}
