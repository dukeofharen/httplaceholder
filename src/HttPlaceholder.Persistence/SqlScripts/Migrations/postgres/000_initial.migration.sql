-- Create stubs table.
create table stubs
(
    id               bigserial
        constraint stubs_pk
            primary key,
    stub_id          varchar(255) not null,
    stub             text         not null,
    stub_type        varchar(20)  not null,
    distribution_key varchar(300) not null default ''
);

create unique index stubs_stub_id_dist_key_uindex
    on stubs (stub_id, distribution_key);

create index stubs_dist_key_index
    on stubs (distribution_key);

-- Create requests table.
create table requests
(
    id                 bigserial
        constraint requests_pk
            primary key,
    correlation_id     varchar(100) not null,
    executing_stub_id  varchar(255) not null,
    request_begin_time timestamp    not null,
    request_end_time   timestamp,
    has_response       boolean,
    json               text         not null,
    distribution_key   varchar(300) not null default ''
);

create index requests_dist_key_index
    on requests (distribution_key);

-- Create responses table.
create table responses
(
    id               bigserial
        constraint responses_pk
            primary key,
    status_code      int          not null,
    headers          text         not null,
    body             text         not null,
    body_is_binary   boolean,
    distribution_key varchar(300) not null default ''
);

create index responses_dist_key_index
    on requests (distribution_key);

alter table responses
    add constraint responses_requests_id_fk
        foreign key (id) references requests;

-- Create metadata table.
create table metadata
(
    id                      bigserial
        constraint metadata_pk
            primary key,
    stub_update_tracking_id varchar(50)
);

-- Create scenarios table.
create table scenarios
(
    id               bigserial
        constraint scenarios_pk
            primary key,
    scenario         varchar(300) not null,
    state            varchar(300) not null,
    hit_count        int          not null,
    distribution_key varchar(300) not null default ''
);

create unique index scenarios_scenario_dist_key_uindex
    on scenarios (scenario, distribution_key);
