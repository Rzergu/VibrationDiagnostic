services:
  registry:
    image: registry:latest
    environment:
      REGISTRY_AUTH: htpasswd
      REGISTRY_AUTH_HTPASSWD_REALM: Registry Realm
      REGISTRY_AUTH_HTPASSWD_PATH: /auth/registry.password
      REGISTRY_STORAGE_FILESYSTEM_ROOTDIRECTORY: /data
    volumes:
      # Mount the password file
      - ./registry/registry.password:/auth/registry.password
      # Mount the data directory
      - ./registry/data:/data
    ports:
      - 5000

user:
HimAdmReg
pass:
Afqwf332@#21scD