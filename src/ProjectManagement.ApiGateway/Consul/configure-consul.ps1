$token = "13585e24-52a3-e906-981a-b2246ed54d77"

consul acl policy create -name="kv-api-gateway" -description="Policy that grants KV access to api-gateway" -rules @api-gateway-policies.hcl -token="$token"
consul acl token create -policy-name="kv-api-gateway" -service-identity="api-gateway" -token="$token"

consul kv put -token="$token" api-gateway/app-config @app-config.json