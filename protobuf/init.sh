#!/bin/bash

bootstrap() {
    mozroots --import --sync
    nuget install Google.ProtocolBuffers
    # TODO: auto extract: "Successfully installed 'Google.ProtocolBuffers 2.4.1.521'."
    PKG="Google.ProtocolBuffers.2.4.1.521"
    ln -s ${PKG}/tools .
    ln -s ${PKG}/lib lib
    chmod +x tools/ProtoGen.exe
    mkdir bin
    cp -r ${PKG}/content/protos/* src/main/proto
    rm -r src/main/proto/tutorial
}

[ -d target ] || bootstrap
