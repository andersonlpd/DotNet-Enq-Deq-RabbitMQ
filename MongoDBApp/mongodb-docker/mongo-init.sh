set -e

mongosh -u $MONGO_INITDB_ROOT_USERNAME -p $MONGO_INITDB_ROOT_PASSWORD <<EOF
    db = db.getSiblingDB('$MONGO_INITDB_ROOT_DATABASE')

    db.createUser({
        user: '$APP_USERNAME',
        pwd: '$APP_PASSWORD',
        roles: [{ role: 'readWrite', db: '$MONGO_INITDB_ROOT_DATABASE' }],
    });
    db.createCollection('$APP_COLLECTION')
EOF