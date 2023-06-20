#!/bin/bash

DXGP_DEB_FILE_PATH="/build-pkg/dxgp-viewer_1.0.0.1_amd64.deb"


rm -rf ./deb-work;

mkdir deb-work;

cp -a deb/* deb-work/
dpkg -x ${DXGP_DEB_FILE_PATH}  deb-work/

dpkg-deb --build ./deb-work ./daasxpert-client.deb
