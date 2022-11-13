;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 13/11/2022 04:09:36 p. m.
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
PRINT "Introduzca el radio del cilindro: "
PRINT "\n"
POP AX
PRINT_NUM
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
