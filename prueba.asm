;Archivo: prueba.cpp
;Fecha: 30/10/2022 02:35:23 p. m.
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
	k DW ?
	l DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
PRINTN "introduce el radio del cilindro "
CALL SCAN_NUM
MOV radio, CX
RET
DEFINE_SCAN_NUM
END
