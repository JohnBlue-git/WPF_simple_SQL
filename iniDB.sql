
IF OBJECT_ID(N'Customer') IS NULL
BEGIN
  CREATE table Customer(
    	  --To specify that the "Personid" column should start at value 10 and increment by 5, change it to IDENTITY(10,5).
          c_id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
          name varchar(30),
          account int,
         );
       
  insert into Customer VALUES('A', 103456);
  --insert into Customer VALUES('B', 303456);--
  insert into Customer VALUES('C', 203456);
  insert into Customer VALUES('D', 603456);
  --insert into Customer VALUES('E', 403456);--
  insert into Customer VALUES('F', 703456);
  insert into Customer VALUES('G', 503456);
  insert into Customer VALUES('H', 803456);
END

IF OBJECT_ID(N'Account') IS NULL
BEGIN
  CREATE table Account(
          a_id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
          account int,
    		  balance int
         );

  insert into Account VALUES(103456, 10);
  insert into Account VALUES(303456, 30);
  --insert into Account VALUES(203456, 20);--
  insert into Account VALUES(603456, 60);
  insert into Account VALUES(403456, 40);
  insert into Account VALUES(703456, 70);
  insert into Account VALUES(503456, 50);
  --insert into Account VALUES(803456, 80);--
END