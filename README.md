# grpc-unity
grpc for unity.

## Install

this package uses the scoped registry feature to resolve package dependencies.
please add the following sections to the manifest file (Packages/manifest.json).

```
{
  "scopedRegistries": [
    {
      "name": "tk-aria",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "com.ariasdk"
      ]
    }
  ],
  "dependencies": {
    "com.ariasdk.grpc-unity": "0.1.0",
    ...
```

<!--
add this package to `manifest.json` (upm for git)
```
"com.ariasdk.grpc-unity" : "https://github.com/tk-aria/grpc-unity.git?path=Assets/GrpcUnity#master"
```
-->

