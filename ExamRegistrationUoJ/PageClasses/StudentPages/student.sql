CREATE FUNCTION getStudentDetails()
RETURNS TABLE(
	id INT,
	name VARCHAR(200),
	email VARCHAR(100)
) A
BEGIN
	RETURN
	 SELECT id,name,email FROM users WHERE id=userId
	 );
END
