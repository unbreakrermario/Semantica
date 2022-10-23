;Archivo: prueba.cpp
;Fecha: 21/10/2022 09:57:42 a. m.
#make_COM#
include emu8086.inc
ORG 100h
;Variable
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
MOV AX, 3
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
MOV AX, 8
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX, 10
PUSH AX
MOV AX, 4
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
MOV y, AX
RET
END
