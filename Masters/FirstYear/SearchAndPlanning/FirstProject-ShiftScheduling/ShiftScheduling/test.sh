#!/bin/sh
sbcl --dynamic-space-size 192 --load test.lisp --non-interactive
grep --color='auto' -P -n "[\x80-\xFF]" G001.lisp
