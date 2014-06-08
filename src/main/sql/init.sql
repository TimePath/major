
-- users and groups

CREATE TABLE groups (
    id serial NOT NULL PRIMARY KEY,
    name text NOT NULL
);

CREATE TABLE users (
    id serial NOT NULL PRIMARY KEY,
    name text NOT NULL
);

CREATE TABLE group_users (
    PRIMARY KEY ( gid, uid ),
    gid int NOT NULL REFERENCES groups ( id ),
    uid int NOT NULL REFERENCES users ( id )
);



-- files

CREATE TABLE descriptors (
    id serial NOT NULL PRIMARY KEY,
-- refer to a file on disk. Should be secret
    uri text NOT NULL UNIQUE,
-- the owning group
    gid int NOT NULL REFERENCES groups ( id ),
-- unix timestamp
    created timestamp NOT NULL
);

CREATE TABLE pathnames (
    id serial NOT NULL PRIMARY KEY,
-- refer to parent. NULL indicates this is a root entry
    parent int REFERENCES pathnames ( id ),
-- the name of the file at this pathname
    name text NOT NULL,
-- refer to a file descriptor containing permissions. NULL indicates this is a folder
    fd int REFERENCES descriptors ( id ),
-- prevent duplicate files in directories
    CONSTRAINT nodupes UNIQUE ( parent, name )
);
-- prevent duplicate files in root directory due to SQL `(NULL != NULL) = NULL` silliness
CREATE UNIQUE INDEX nodupes_root ON pathnames (name) WHERE parent IS NULL;



CREATE OR REPLACE FUNCTION numch(text, text) RETURNS integer AS $$
SELECT length($2) - length(replace($2, $1, '')) $$ LANGUAGE SQL;



-- `CREATE VIEW mappings AS` for caching?
CREATE OR REPLACE FUNCTION tree( maxdepth int = 1 ) RETURNS TABLE( id int, name text, parent int, depth int, path text ) AS $$
BEGIN
RETURN QUERY
WITH RECURSIVE pathnames_cte( id, name, parent, depth, path ) AS (
    SELECT root.id, root.name, root.parent, 1::int as depth, ( '/' || root.name::TEXT ) AS path
    FROM pathnames AS root
    WHERE root.parent IS NULL
    UNION ALL
    SELECT child.id, child.name, child.parent, parent.depth + 1, ( parent.path || '/' || child.name::TEXT )
    FROM pathnames_cte AS parent, pathnames AS child
    WHERE child.parent = parent.id AND parent.depth < maxdepth
)
SELECT *
FROM pathnames_cte AS path
ORDER BY path.id ASC;
END; $$
LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION children( path_request text ) RETURNS TABLE( id int, name text ) AS $$
DECLARE
        parent_id int;
BEGIN
    IF length(path_request) < 2 THEN
        RETURN QUERY SELECT root.id, root.name FROM tree() AS root;
    ELSE

        SELECT tree.id INTO parent_id
        FROM tree(numch('/', path_request)) AS tree
        WHERE tree.path = path_request
        LIMIT 1;

        RETURN QUERY SELECT path.id, path.name FROM pathnames AS path where path.parent = parent_id;
    END IF;
END; $$
LANGUAGE plpgsql;
