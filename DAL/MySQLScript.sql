DROP SCHEMA IF EXISTS Students;
CREATE SCHEMA Students;
USE Students;

CREATE TABLE student(
	id bigint AUTO_INCREMENT NOT NULL Primary Key,
	name varchar(500) NOT NULL,
	type varchar(10) NOT NULL,
	gender char(1) NOT NULL,
	enabled bit NOT NULL,
	updated_on datetime NOT NULL
) ENGINE = InnoDB;

INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Anthony "Tony" Edward Stark','University','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Thor Odinson','University','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Dr. Henry Jonathan "Hank" Pym','University','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Janet van Dyne','University','F',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Dr. Robert Bruce Banner','University','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Steven "Steve" Rogers','High','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Clinton Francis Barton','Elementary','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Pietro Maximoff','Kinder','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Wanda Maximoff','Kinder','F',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('T''Challa','University','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Natasha Alianovna Romanoff','Elementary','F',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('Samuel "Snap" Thomas Wilson','Elementary','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES ('James Rupert "Rhodey" Rhodes','Kinder','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES('Miles Morales','Kinder','M',1,NOW());
INSERT INTO student (name,type,gender,enabled,updated_on) VALUES('Wade Wilson','Kinder','M',1,NOW());

DELIMITER ;;
CREATE FUNCTION addStudent(  
	pname varchar(500),
	ptype varchar(10),
	pgender char(1) 
) RETURNS bigint
BEGIN  
	
    INSERT INTO student (name,type,gender,enabled,updated_on) 
	SELECT pname, ptype, pgender, 1, NOW();
 
    RETURN last_insert_id();
END ;;

CREATE FUNCTION addFullStudent(  
	pname varchar(500),
	ptype varchar(10),
	pgender char(1), 
    penabled bit,
    pupdated_on datetime 
) RETURNS bigint
BEGIN
	
    INSERT INTO student (name,type,gender,enabled,updated_on) 
	SELECT pname, ptype, pgender, penabled, pupdated_on;
    
    RETURN last_insert_id();
END ;;

CREATE PROCEDURE delStudent( 
    pid bigint 
)
BEGIN   
    START TRANSACTION; 
 
    DELETE 
	FROM   student 
	WHERE  id = pid; 
 
    COMMIT; 
END ;;

CREATE PROCEDURE hideStudent( 
    pid bigint 
)
BEGIN   
    START TRANSACTION; 
	
    UPDATE student 
	SET    enabled = 0, updated_on = NOW() 
	WHERE  id = pid; 
 
    COMMIT; 
END ;;

CREATE PROCEDURE updStudent( 
	IN pid bigint,
	IN pname varchar(500),
	IN ptype varchar(10),
	IN pgender char(1), 
    IN penabled bit
)
BEGIN  
    START TRANSACTION; 
 
    UPDATE student 
	SET  name = pname,type=ptype,gender=pgender,enabled=penabled,updated_on=NOW() 
	WHERE  id = pid; 
 
    COMMIT; 
    
    SELECT id,name,type,gender,enabled,updated_on 
	FROM   student 
	WHERE  id = pid;  
    
END ;;

CREATE PROCEDURE selStudentById(
	IN pid BIGINT
)
BEGIN  
    START TRANSACTION; 
 
	SELECT id,name,type,gender,enabled,updated_on  
	FROM   student
	WHERE  id = pid;
 
    COMMIT; 
END ;;

CREATE PROCEDURE selPaginatedStudentsWhere(
    IN pOFFSET BIGINT,
	IN pLIMIT BIGINT,
	OUT pCOUNT BIGINT,
	OUT pTPAGS BIGINT, 
	IN pfield varchar(10),
	IN pcond varchar(2000),
	IN psort varchar(4),
	IN penabled bit
)
BEGIN
	SET pOFFSET = (pOFFSET - 1) * pLIMIT;

	IF pcond <> '' Then 
		SET pcond = concat(' AND ', pcond); 
	ELSE
		SET pcond = ''; 
	END IF;
        
	SET @query = CONCAT('SELECT id,name,type,gender,enabled,updated_on FROM student WHERE enabled = ? ',pcond,' order by ',pfield,' ',psort,' LIMIT ?, ?');

	SET @enabled = penabled;
	SET @OFFSET = pOFFSET;
	SET @LIMIT = pLIMIT;
    SET @COUNT = 0;

	PREPARE stmt FROM @query;
	EXECUTE stmt using @enabled, @OFFSET, @LIMIT;
	DEALLOCATE PREPARE stmt;

	SET @query = CONCAT('SELECT COUNT(*) INTO @COUNT FROM student WHERE enabled = ? ',pcond);

	PREPARE stmt FROM @query;
	EXECUTE stmt using @enabled;
    select @COUNT into pCOUNT;
	DEALLOCATE PREPARE stmt;

	IF pCOUNT % pLIMIT > 0 THEN
		SET pTPAGS = 1;
	ELSE
		SET pTPAGS = 0;
	END IF;

	SET pTPAGS = pTPAGS + ROUND(pCOUNT / pLIMIT, 0);
END ;;

DELIMITER ;