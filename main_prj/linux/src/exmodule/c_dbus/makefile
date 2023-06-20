#
# Makefile
#

all: c_dbus

%.o: %.c
	gcc -shared -fPIC -Wall -c $< `pkg-config --cflags dbus-1`

c_dbus: c_dbus.o
	gcc -shared -fPIC c_dbus.o -o c_dbus.so `pkg-config --libs dbus-1`

.PHONY: clean
clean:
	rm *.o *.so 

