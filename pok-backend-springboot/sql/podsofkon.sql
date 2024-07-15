
create table scores (PLAYERNAME       VARCHAR2(256) ,
             SCORE            NUMBER(10) )

ALTER TABLE scores ADD created_at TIMESTAMP;

CREATE OR REPLACE TRIGGER add_timestamp_for_score
BEFORE INSERT ON scores
FOR EACH ROW
BEGIN
  :NEW.created_at := SYSTIMESTAMP;
END;
/

