;Archivo: prueba.cpp
;Fecha: 25/10/2022 09:49:52 a. m.
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
MOV AX, 61
PUSH AX
POP AX
MOV y, AX
MOV AX, 60
PUSH AX
MOV AX, 61
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if1
MOV AX, 10
PUSH AX
POP AX
MOV x, AX
if1:
RET
END
