;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 11/11/2022 09:49:29 a. m.
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
PRINT "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura, CX
