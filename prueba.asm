;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 08/11/2022 11:30:56 p. m.
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
	cinco DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
	k DW ?
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if1
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
