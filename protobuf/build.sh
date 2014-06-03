#!/bin/bash
PROTO_GEN=$(realpath tools/ProtoGen.exe)
PROTOC_DIR=$(realpath bin)

mkdir -p target
JAVA=$(realpath target/java)
CSHARP=$(realpath target/csharp)
mkdir -p ${JAVA}
mkdir -p ${CSHARP}

cd src/main/proto

# Protobuf doesn't provide source/binary compatibility across different versions
# TODO: -ignore_google_protobuf=true doesn't work properly

FOUND=$(find . -type f -name '*.proto')
echo Compiling: ${FOUND}

${PROTO_GEN} --protoc_dir=${PROTOC_DIR} --proto_path=. \
--include_imports \
--java_out=${JAVA} -output_directory=${CSHARP} \
${FOUND}
