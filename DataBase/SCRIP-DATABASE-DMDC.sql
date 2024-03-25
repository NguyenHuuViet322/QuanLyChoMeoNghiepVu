--SCRIP CREATE PROCEDURE
-- PROCEDURE CREATE SCHEMA
create or replace NONEDITIONABLE PROCEDURE prc_job_create_schema(
	schema_name NVARCHAR2,
    schema_pass NVARCHAR2,
    schema_role NVARCHAR2
) AS
    v_sql       VARCHAR2(4000);
	PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
v_sql:= 'alter session set "_ORACLE_SCRIPT"=true';  
EXECUTE IMMEDIATE v_sql;
v_sql:='CREATE USER '||schema_name||' IDENTIFIED BY '|| schema_pass;
EXECUTE IMMEDIATE v_sql;
v_sql:= ' GRANT CONNECT, RESOURCE, DBA TO '||schema_name;
EXECUTE IMMEDIATE v_sql;
v_sql:= ' GRANT CREATE TABLE TO '||schema_name;
EXECUTE IMMEDIATE v_sql;
v_sql:= 'GRANT SELECT ANY TABLE, INSERT ANY TABLE, DELETE ANY TABLE, UPDATE ANY TABLE TO  '||schema_name;
EXECUTE IMMEDIATE v_sql;
END;
-- -- PROCEDURE CREATE TABLE
create or replace NONEDITIONABLE PROCEDURE prc_job_create_table(
	schema_name NVARCHAR2,
    table_name NVARCHAR2,
    table_column NVARCHAR2
) AS
    v_sql       VARCHAR2(4000);
	PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
v_sql:= 'alter session set "_ORACLE_SCRIPT"=true';  
EXECUTE IMMEDIATE v_sql;
v_sql:='CREATE TABLE '||schema_name||'.'||table_name||' ('||table_column||' )';
EXECUTE IMMEDIATE v_sql;
END;

-- -- PROCEDURE ALTER TABLE ( THÊM, XÓA CỘT TRONG BẢNG)
create or replace NONEDITIONABLE PROCEDURE prc_job_alter_table(
	schema_name NVARCHAR2,
    table_name NVARCHAR2,
    column_name NVARCHAR2,
    column_datatype NVARCHAR2,
    type_action NVARCHAR2
) AS
    v_sql       VARCHAR2(4000);
	PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN

IF(type_action='ADDNEW') THEN
v_sql:='ALTER TABLE '||schema_name||'.'||table_name||' ADD '||column_name||' '||column_datatype;
EXECUTE IMMEDIATE v_sql;
ELSIF (type_action='DELETE') THEN
v_sql:='ALTER TABLE '||schema_name||'.'||table_name||' DROP COLUMN  '||column_name;
EXECUTE IMMEDIATE v_sql;
END IF;
END;
/

-- SQL TO CALL CREATE SCHEMA
CALL prc_job_create_schema('SCHEMANAME','PASSWORD1',NULL);
-- SQL TO CALL CREATE TABLE
CALL prc_job_create_table('SCHEMANAME','TABLE1','ID NUMBER(10,0) GENERATED ALWAYS AS IDENTITY MINVALUE 1 MAXVALUE 9999999999 INCREMENT BY 1 START WITH 283 CACHE 20 NOORDER  NOCYCLE  NOT NULL ENABLE, 
COLUM_TEST VARCHAR(255),COLUM_TEST2 VARCHAR(255), CONSTRAINT "ID_TABLE1_PK" PRIMARY KEY ("ID")');
-- SQL TO CALL ALTER TABLE ( THÊM, XÓA CỘT TRONG BẢNG)
CALL prc_job_alter_table('SCHEMANAME','TABLE1','COLUM_TEST2','VARCHAR(255)','ADDNEW'); -- THÊM CỘT
CALL prc_job_alter_table('SCHEMANAME','TABLE1','COLUM_TEST','VARCHAR(255)','DELETE'); -- XÓA CỘT



