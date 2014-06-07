#!/bin/bash
# Responsible for building csharp protobuf messages

PROTOGEN=$(realpath tools/ProtoGen.exe)
PROTOC_DIR=$(realpath bin)

CSHARP=$(realpath target/generated-sources/csharp)
mkdir -p ${CSHARP}

cd src/main/proto

FOUND=$(find . -type f -name '*.proto')
echo Compiling: ${FOUND}

${PROTOGEN} --protoc_dir=${PROTOC_DIR} -ignore_google_protobuf=true --proto_path=. --include_imports -output_directory=${CSHARP} ${FOUND}

# The double colon operator with using alias directive on generic types confuses monodevelop
sed -e 's/::/./g' -e 's/global./global::/g' -i -s $(find ${CSHARP} -type f -name '*.cs')
