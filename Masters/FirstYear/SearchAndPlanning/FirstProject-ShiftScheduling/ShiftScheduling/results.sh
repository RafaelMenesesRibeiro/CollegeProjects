#!/bin/sh
sbcl --dynamic-space-size 192 --load results.lisp --non-interactive | tee results.txt
