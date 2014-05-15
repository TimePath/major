#!/bin/bash
[ -d target ] || bootstrap

bootstrap() {
    mozroots --import --sync
    nuget install Google.ProtocolBuffers
    # TODO: auto extract: "Successfully installed 'Google.ProtocolBuffers 2.4.1.521'."
    PKG="Google.ProtocolBuffers.2.4.1.521"
    ln -s ${PKG}/tools tools
    ln -s ${PKG}/lib lib
    chmod +x tools/ProtoGen.exe
    mkdir bin
    ln -s $(which protoc) bin/protoc.exe
    cp -r ${PKG}/content/protos src/main
    rm -r src/main/protos/tutorial
}
