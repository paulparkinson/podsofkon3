
create table scores (PLAYERNAME       VARCHAR2(256) ,  SCORE            NUMBER(10) )
ALTER TABLE scores ADD created_at TIMESTAMP;
CREATE OR REPLACE TRIGGER add_timestamp_for_score
BEFORE INSERT ON scores
FOR EACH ROW
BEGIN
  :NEW.created_at := SYSTIMESTAMP;
END;
/


SELECT
    s.playername,
    s.score,
    s.created_at,
    p.firstname,
    p.lastname,
    p.email
FROM
    ADMIN.SCORES s
JOIN
    ADMIN.PLAYERINFO p ON s.playername = p.playername
WHERE
    TRUNC(s.created_at) = TRUNC(SYSDATE)
ORDER BY
    s.score DESC;
