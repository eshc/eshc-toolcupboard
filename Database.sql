CREATE DATABASE tool_catalogue
  WITH OWNER = tool_catalogue
  ENCODING = 'UTF8'
  TABLESPACE = pg_default
  LC_COLLATE = 'en_US.UTF-8'
  LC_CTYPE = 'en_US.UTF-8'
  CONNECTION LIMIT = -1;

DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;


CREATE TABLE public.access_log
  (
    access_id bigserial PRIMARY KEY NOT NULL,
    date timestamp NOT NULL,
    card_id text NOT NULL
  );
ALTER TABLE public.access_log
  OWNER TO tool_catalogue;

CREATE TABLE public.users
  (
    user_id bigserial PRIMARY KEY NOT NULL,
    login text NOT NULL,
    "name" text NOT NULL,
    "email" text NOT NULL,
    added timestamp NOT NULL
  );
ALTER TABLE public.users
  OWNER TO tool_catalogue;

CREATE TABLE public.user_cards
  (
    card_id text PRIMARY KEY NOT NULL,
    description text NOT NULL,
    user_id bigint NOT NULL REFERENCES users(user_id),
    added timestamp NOT NULL
  );
ALTER TABLE public.user_cards
  OWNER TO tool_catalogue;

CREATE TABLE public.tools
  (
    tool_id bigserial PRIMARY KEY NOT NULL,
    "name" text NOT NULL,
    description text NOT NULl,
    location text NOT NULL,
    added timestamp NOT NULL
  );
ALTER TABLE public.tools
  OWNER TO tool_catalogue;

CREATE TABLE public.tool_cards
  (
    card_id text PRIMARY KEY NOT NULL,
    description text NOT NULL,
    tool_id bigint NOT NULL REFERENCES tools(tool_id),
    added timestamp NOT NULL
  );
ALTER TABLE public.tool_cards
  OWNER TO tool_catalogue;

CREATE TABLE public.tool_checkout
  (
    tool_id bigint PRIMARY KEY NOT NULL REFERENCES tools(tool_id),
    user_id bigint NOT NULL REFERENCES users(user_id),
    since timestamp NOT NULL
  );
ALTER TABLE public.tool_checkout
  OWNER TO tool_catalogue;

CREATE TABLE public.user_log
  (
    access_id bigserial PRIMARY KEY NOT NULL,
    user_id bigint NOT NULL REFERENCES users(user_id),
    card_id text NOT NULL,
    date timestamp NOT NULL
  );
ALTER TABLE public.user_log
  OWNER TO tool_catalogue;

CREATE TABLE public.tool_log
  (
    access_id bigserial PRIMARY KEY NOT NULL,
    date timestamp NOT NULL,
    user_id bigint NOT NULL REFERENCES users(user_id),
    user_card_id text NOT NULL,
    tool_id bigint NOT NULl REFERENCES tools(tool_id),
    tool_card_id text NOT NULL,
    check_in boolean NOT NULl
  );
ALTER TABLE public.tool_log
  OWNER TO tool_catalogue;



