set -e

mongosh -u $MONGO_INITDB_ROOT_USERNAME -p $MONGO_INITDB_ROOT_PASSWORD <<EOF
    db = db.getSiblingDB('rabbit')

    db.createUser({
        user: 'rabbit',
        pwd: '$RABBIT_PASSWORD',
        roles: [{ role: 'readWrite', db: 'rabbit' }],
    });
    db.createCollection('messages')
EOF