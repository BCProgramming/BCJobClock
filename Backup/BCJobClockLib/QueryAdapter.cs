using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{
    //after learning that queries were actually different in SQL Server and MySQL, I decided to define an interface
    //rather than have the queries (and more precisely, query "formats" directly in the code, it will call into the query adapter
    //selected at startup based on the selected database engine.


    public abstract class QueryAdapter
    {
        protected Dictionary<String, String> Qlookup = new Dictionary<string, string>();
        

        protected QueryAdapter()
        {
            Qlookup = new Dictionary<string, string>();

        }

        public string this[String index]
        {
            get {
                return GetQuery(index);
            
            }

        }
        public  virtual string GetQuery(string QueryIdentifier)
        {
            if (!Qlookup.ContainsKey(QueryIdentifier))
            {
                Debug.Print("No " + QueryIdentifier + " Present!");
                Debug.Assert(false, "No " + QueryIdentifier + " Present!");
                return null;
            }
            else
            {


                return Qlookup[QueryIdentifier];
            }
        }
    }


    public class MySQLQueryAdapter : QueryAdapter
    {

        public MySQLQueryAdapter():base()
        {
            Qlookup.Add("DBTIME", "SELECT NOW()");
            Qlookup.Add("PINLOOKUP", "SELECT * FROM USERS WHERE PINCode=\u0022{0}\u0022");
            Qlookup.Add("USERPIN", "SELECT * FROM USERS WHERE UserName=\u0022{0}\u0022 LIMIT 1");
            Qlookup.Add("ADDUSERFMT", "INSERT INTO USERS (UserName, PINCode) VALUES (\u0022{0}\u0022,\u0022{1}\u0022)");
            Qlookup.Add("DELETEUSRFMT", "DELETE * FROM USERS WHERE UserName=\u0022{0}\u0022");
            Qlookup.Add("CLEARUSERORDERS", "DELETE * FROM ORDERDATA WHERE UserPinCode=\u0022{0}\u0022");
            Qlookup.Add("INSERTCLOCKED", "INSERT INTO CLOCKED (`UserPIN`,`OrderID`,`EventType`) VALUES (\"{0}\",\"{1}\",\"{2}\")");
            Qlookup.Add("CHECKUSERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND `Order`=\"{1}\"");
            Qlookup.Add("UPDATETOTALTIMEFMT", "UPDATE OrderData SET TotalTime=\u0022{0}\u0022, StartTime=NULL WHERE UserPinCode=\u0022{1}\u0022 AND `Order`=\u0022{2}\u0022");
            Qlookup.Add("UPDATECLOCKOUTFMT", "UPDATE OrderData SET LastClockOut=NOW() WHERE UserPINCode=\"{0}\"");
            Qlookup.Add("LASTORDERCLOCKFMT", "SELECT * FROM OrderData WHERE `Order`=\"{0}\" AND LastClockOut IS NOT NULL");
            Qlookup.Add("LASTCLOCKOUTFORUSERORDERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND LastClockOut IS NOT NULL");
            Qlookup.Add("CHECKUSERORDERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\u0022{0}\u0022 AND `Order`=\u0022{1}\u0022");
            Qlookup.Add("CLOCKINSQLFMT", "INSERT INTO OrderData (UserPinCode,StartTime,TotalTime,`Order`) VALUES (\u0022{0}\u0022,\u0022{1}\u0022,\u0022{2}\u0022,\u0022{3}\u0022)");
            Qlookup.Add("QUERYORDERS", "SELECT * From TheOrders");
            Qlookup.Add("ADDORDER", "INSERT INTO TheOrders (OrderID) VALUES (\"{0}\")");
            Qlookup.Add("ACTIVEJOBCOUNTUSERFMT", "SELECT COUNT(*) FROM OrderData WHERE UserPinCode = \"{0}\" AND StartTime IS NOT NULL");
            Qlookup.Add("SELECTORDERFMT", "SELECT * FROM OrderData WHERE `Order`=\"{0}\"");
            Qlookup.Add("ORDERBYUSER", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\"");
            Qlookup.Add("ALLUSERS", "SELECT * FROM USERS");
            Qlookup.Add("PURGELOG", "DELETE * FROM `MESSAGELOG`");
            Qlookup.Add("ADDMESSAGE", "INSERT INTO `MESSAGELOG` (`LogID`,`Type`,`Message`) VALUES (\"{0}\",\"{1}\",\"{2}\")");
            Qlookup.Add("GETLOGIDFMT", "SELECT * FROM MESSAGELOG WHERE `LogID`=\"{0}\"");
            Qlookup.Add("ORDERDESCRIPTIONFMT", "SELECT `Description` FROM TheOrders WHERE `OrderID`=\"{0}\"");
            Qlookup.Add("ALLORDERIDS", "SELECT DISTINCT `Order` FROM OrderData");
            Qlookup.Add("DISTINCTORDERDATA", "SELECT DISTINCT `OrderID` FROM TheOrders");

            Qlookup.Add("USERDATAFROMORDER", "SELECT * FROM OrderData INNER JOIN USERS ON OrderData.UserPinCode=USERS.PINCode WHERE `Order`=\u0022{0}\u0022");

            Qlookup.Add("USERTOTALTIMEQUERY", "SELECT * FROM CLOCKED WHERE `UserPIN`=\"{0}\" AND `OrderID`=\"{1}\"");
            Qlookup.Add("WORKORDERDATE", "SELECT FirstDate FROM OrderData WHERE `Order`=\"{0}\" ORDER BY FirstDate ASC LIMIT 1");
            Qlookup.Add("ACTIVEWORKERS", "SELECT UserPinCode FROM OrderData where `Order`=\"{0}\" AND StartTime IS NOT NULL");
            Qlookup.Add("USERNAMEFROMPIN", "SELECT UserName FROM USERS WHERE PINCode=\"{0}\"");
            Qlookup.Add("ORDERCLOCKED", "SELECT * FROM CLOCKED WHERE `UserPIN`=\"{0}\" AND `OrderID`=\"{1}\" ORDER BY `STAMP` ASC");
            Qlookup.Add("USERORDERQUERYACTIVE", "SELECT DISTINCT `Order` FROM OrderData WHERE UserPinCode=\u0022{0}\u0022 AND StartTime IS NOT NULL");
            Qlookup.Add("USERORDERQUERYALL", "SELECT DISTINCT `Order` FROM OrderData WHERE UserPinCode=\u0022{0}\u0022");
            Qlookup.Add("NUMACTIVEORDERS", "SELECT COUNT(*) FROM OrderData WHERE UserPinCode=\"{0}\" AND StartTime IS NULL");
            Qlookup.Add("GETMESSAGELOG", "SELECT * FROM MESSAGELOG");
            Qlookup.Add("GETUSERS", "SELECT * FROM USERS");
            Qlookup.Add("INSERTORDERFMT", "INSERT INTO TheOrders (OrderID,Description) VALUES (\"{1}\",\"{2}\")");
            Qlookup.Add("UPDATEORDERFMT", "UPDATE TheOrders SET OrderID=\"{1}\", Description=\"{2}\" WHERE OrderID=\"{0}\"");
            Qlookup.Add("UPDATEORDERIDFMT", "UPDATE USERS SET OrderData=\"{0}\" WHERE OrderData=\"{1}\"");
            Qlookup.Add("GETRECORDID", "SELECT RecordID FROM USERS WHERE UserName=\"{0}\" AND PINCode=\"{1}\"");
            Qlookup.Add("UPDATEPINCODEFMT", "UPDATE OrderData SET PINCode=\"{1}\" WHERE PINCode=\"{0}\"");
            Qlookup.Add("USERNAMEFROMPINFMT", "SELECT UserName FROM USERS WHERE PinCode=\"{0}\"");
            Qlookup.Add("DELONUSERPINFMT", "DELETE FROM USERS WHERE UserName=\"{0}\" AND PINCode=\"{1}\"");
            Qlookup.Add("FROMRECORDIDFMT", "SELECT UserName,PINCode FROM USERS WHERE `RecordID`=\"{0}\"");
            Qlookup.Add("DROPMESSAGES", "DELETE FROM MESSAGELOG");
            Qlookup.Add("LATESTSTARTUSERORDER", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND `Order`=\"{1}\"");

            //and now, the table building code.

            Qlookup.Add("DROPDBIFEXISTSFMT", @"DROP DATABASE IF EXISTS `{0}`");
            Qlookup.Add("CREATEDBFMT", @"CREATE DATABASE `{0}` ; USE `{0}`");


            Qlookup.Add("CREATEORDERDATATABLE",
@"DROP TABLE IF EXISTS `OrderData`;
CREATE TABLE `OrderData` (
  `WorkID` int(11) NOT NULL AUTO_INCREMENT,
  `UserPinCode` text NOT NULL COMMENT 'PIN Code of the user for this Work Order',
  `TotalTime` bigint(20) NOT NULL COMMENT 'Total time (in ms) this user has spent on this order',
  `StartTime` datetime DEFAULT NULL COMMENT 'Starting time of current workspan, or null if user is not clocked in.',
  `Order` VARCHAR(256) NOT NULL COMMENT 'Order ID.',
  `FirstDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Set when the record is created.',
  `LastClockOut` timestamp NULL DEFAULT NULL COMMENT 'Contains the TIMESTAMP corresponding to the last time this Work Order was clocked out from.',
  PRIMARY KEY (`WorkID`)
) ;");

            Qlookup.Add("CREATEORDERSTABLE", 
@"DROP TABLE IF EXISTS `TheOrders`;
CREATE TABLE `TheOrders` (
  ID int(11) NOT NULL AUTO_INCREMENT,
  `OrderID` text NOT NULL,
  `Description` text NOT NULL,
PRIMARY KEY(`ID`)
);");
            Qlookup.Add("CREATEUSERSTABLE", 
@"DROP TABLE IF EXISTS `USERS`;
CREATE TABLE `USERS` (
  `RecordID` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(256) NOT NULL,
  `PINCode` varchar(64) NOT NULL,
  `ACTIVE` TINYINT NOT NULL DEFAULT `1`
  PRIMARY KEY (`RecordID`)
);");
            
            Qlookup.Add("USERSACTIVEFIELDQUERY",
                @"SHOW COLUMNS FROM `USERS` WHERE FIELD=`ACTIVE`");


            
            Qlookup.Add("ADDACTIVEFIELDTOUSERS",
                @"ALTER TABLE `USERS` ADD `ACTIVE` TINYINT NOT NULL DEFAULT '1' AFTER `RecordID`"

                );
            Qlookup.Add("CREATEMESSAGELOGTABLE", 
@"DROP TABLE IF EXISTS `MESSAGELOG`;
CREATE TABLE `MESSAGELOG` (
`ID` int(11) NOT NULL AUTO_INCREMENT,
`LogID` varchar(64) NOT NULL DEFAULT 'GENERAL',
`Type` varchar(64) NOT NULL,
`Tstamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Set when record is created. identifies the Timestamp of this message.',
`Message` text NOT NULL COMMENT 'Message Text.',
PRIMARY KEY (`ID`)
) ;");

            Qlookup.Add("SETUSERACTIVE", "UPDATE `USERS` SET ACTIVE=\"{1}\" WHERE `PINCode`=\"{0}\"");

            Qlookup.Add("GETACTIVEUSERS",
                "SELECT * FROM `USERS` WHERE ACTIVE=\"{0}\"");

            Qlookup.Add("GETUSERACTIVE",
                "SELECT `ACTIVE` FROM `USERS` WHERE `PINCode`=\"{0}\"");
            Qlookup.Add("CREATECLOCKEDTABLE",
@"DROP TABLE IF EXISTS `CLOCKED`;
CREATE TABLE `CLOCKED` (
`ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID of this record.',
`UserPIN` varchar(64) NOT NULL COMMENT 'PIN of the user clocking in or out',
`OrderID` varchar(64) NOT NULL COMMENT 'Order ID being clocked into or out of',
`STAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Timestamp of this record.',
`EventType` varchar(32) NOT NULL COMMENT 'Type of the event. usually IN or OUT to indicate clocking in or out respectively.',
PRIMARY KEY (`ID`)
);");
            Qlookup.Add("UPDATECONFIGSETTING", "UPDATE `CONFIG` SET `Value`=\"{0}\" WHERE `SectionName`=\"{1}\" AND `ValueName`=\"{2}\"");
            Qlookup.Add("INSERTCONFIGSETTING", "INSERT INTO `CONFIG` (`SectionName`,`ValueName`,`Value`) VALUES (\"{0}\",\"{1}\",\"{2}\")");
            Qlookup.Add("GETCONFIGSETTING", "SELECT * FROM CONFIG WHERE `SectionName`=\"{0}\" AND `ValueName`=\"{1}\"");
            Qlookup.Add("CREATECONFIGTABLE",
                @"DROP TABLE IF EXISTS `CONFIG`;
CREATE TABLE `CONFIG` (
`ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID of this record.',
`SectionName` varchar(128) NOT NULL COMMENT 'Section Name of this config entry.',
`ValueName` varchar(128) NOT NULL COMMENT 'Value Name of this configuration entry.',
`Value` TEXT NOT NULL COMMENT 'The value itself.',
PRIMARY KEY(`ID`)
);");
        }

    }

    public class SQLServerQueryAdapter : QueryAdapter
    {

        public SQLServerQueryAdapter()
            : base()
        {
            Qlookup.Add("DBTIME", "SELECT GETDATE()");
            Qlookup.Add("PINLOOKUP", "SELECT * FROM USERS WHERE PINCode=\u0022{0}\u0022");
            Qlookup.Add("USERPIN", "SELECT TOP 1 * FROM USERS WHERE UserName=\u0022{0}\u0022");
            Qlookup.Add("ADDUSERFMT", "INSERT INTO USERS (UserName, PINCode) VALUES (\u0022{0}\u0022,\u0022{1}\u0022)");
            Qlookup.Add("DELETEUSRFMT", "DELETE * FROM USERS WHERE UserName=\u0022{0}\u0022");
            Qlookup.Add("CLEARUSERORDERS", "DELETE * FROM ORDERDATA WHERE UserPinCode=\u0022{0}\u0022");
            Qlookup.Add("INSERTCLOCKED", "INSERT INTO CLOCKED (`UserPIN`,`OrderID`,`EventType`) VALUES (\"{0}\",\"{1}\",\"{2}\")");
            Qlookup.Add("CHECKUSERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND `Order`=\"{1}\"");
            Qlookup.Add("UPDATETOTALTIMEFMT", "UPDATE OrderData SET TotalTime=\u0022{0}\u0022, StartTime=NULL WHERE UserPinCode=\u0022{1}\u0022 AND `Order`=\u0022{2}\u0022");
            Qlookup.Add("UPDATECLOCKOUTFMT", "UPDATE OrderData SET LastClockOut=NOW() WHERE UserPINCode=\"{0}\"");
            Qlookup.Add("LASTORDERCLOCKFMT", "SELECT * FROM OrderData WHERE `Order`=\"{0}\" AND LastClockOut IS NOT NULL");
            Qlookup.Add("LASTCLOCKOUTFORUSERORDERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND LastClockOut IS NOT NULL");
            Qlookup.Add("CHECKUSERORDERFMT", "SELECT * FROM OrderData WHERE UserPinCode=\u0022{0}\u0022 AND `Order`=\u0022{1}\u0022");
            Qlookup.Add("CLOCKINSQLFMT", "INSERT INTO OrderData (UserPinCode,StartTime,TotalTime,`Order`) VALUES (\u0022{0}\u0022,\u0022{1}\u0022,\u0022{2}\u0022,\u0022{3}\u0022)");
            Qlookup.Add("QUERYORDERS", "SELECT * From TheOrders");
            Qlookup.Add("ADDORDER", "INSERT INTO TheOrders (OrderID) VALUES (\"{0}\")");
            Qlookup.Add("ACTIVEJOBCOUNTUSERFMT", "SELECT COUNT(*) FROM OrderData WHERE UserPinCode = \"{0}\" AND StartTime IS NOT NULL");
            Qlookup.Add("SELECTORDERFMT", "SELECT * FROM OrderData WHERE `Order`=\"{0}\"");
            Qlookup.Add("ORDERBYUSER", "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\"");
            Qlookup.Add("ALLUSERS", "SELECT * FROM USERS");
            Qlookup.Add("ADDMESSAGE", "INSERT INTO `MESSAGELOG` (`LogID`,`Type`,`Message`) VALUES (\"{0}\",\"{1}\",\"{2}\")");
            Qlookup.Add("GETLOGIDFMT", "SELECT * FROM MESSAGELOG WHERE `LogID`=\"{0}\"");
            Qlookup.Add("ORDERDESCRIPTIONFMT", "SELECT `Description` FROM TheOrders WHERE `OrderID`=\"{0}\"");
            Qlookup.Add("ALLORDERIDS", "SELECT DISTINCT `Order` FROM OrderData");
            Qlookup.Add("DISTINCTORDERDATA", "SELECT DISTINCT `OrderID` FROM TheOrders");
            Qlookup.Add("USERDATAFROMORDER", "SELECT * FROM OrderData INNER JOIN USERS ON OrderData.UserPinCode=USERS.PINCode WHERE `Order`=\u0022{0}\u0022");
            Qlookup.Add("USERTOTALTIMEQUERY", "SELECT * FROM CLOCKED WHERE `UserPIN`=\"{0}\" AND `OrderID`=\"{1}\"");
            Qlookup.Add("WORKORDERDATE", "SELECT TOP 1 FirstDate FROM OrderData WHERE `Order`=\"{0}\" ORDER BY FirstDate ASC");
            Qlookup.Add("ACTIVEWORKERS", "SELECT UserPinCode FROM OrderData where `Order`=\"{0}\" AND StartTime IS NOT NULL");
            Qlookup.Add("USERNAMEFROMPIN", "SELECT UserName FROM USERS WHERE PINCode=\"{0}\"");
            Qlookup.Add("ORDERCLOCKED", "SELECT * FROM CLOCKED WHERE `UserPIN`=\"{0}\" AND `OrderID`=\"{1}\" ORDER BY `STAMP` ASC");
            Qlookup.Add("USERORDERQUERYACTIVE", "SELECT DISTINCT `Order` FROM OrderData WHERE UserPinCode=\u0022{0}\u0022 AND StartTime IS NOT NULL");
            Qlookup.Add("USERORDERQUERYALL", "SELECT DISTINCT `Order` FROM OrderData WHERE UserPinCode=\u0022{0}\u0022");
            Qlookup.Add("NUMACTIVEORDERS", "SELECT COUNT(*) FROM OrderData WHERE UserPinCode=\"{0}\" AND StartTime IS NULL");
            Qlookup.Add("GETMESSAGELOG", "SELECT * FROM MESSAGELOG");
            Qlookup.Add("GETUSERS", "SELECT * FROM USERS");
            Qlookup.Add("INSERTORDERFMT", "INSERT INTO TheOrders (OrderID,Description) VALUES (\"{1}\",\"{2}\")");
            Qlookup.Add("UPDATEORDERFMT", "UPDATE TheOrders SET OrderID=\"{1}\", Description=\"{2}\" WHERE OrderID=\"{0}\"");
            Qlookup.Add("UPDATEORDERIDFMT", "UPDATE USERS SET OrderData=\"{0}\" WHERE OrderData=\"{1}\"");
            Qlookup.Add("GETRECORDID", "SELECT RecordID FROM USERS WHERE UserName=\"{0}\" AND PINCode=\"{1}\"");
            Qlookup.Add("UPDATEPINCODEFMT", "UPDATE OrderData SET PINCode=\"{1}\" WHERE PINCode=\"{0}\"");
            Qlookup.Add("USERNAMEFROMPINFMT", "SELECT UserName FROM USERS WHERE PinCode=\"{0}\"");
            Qlookup.Add("DELONUSERPINFMT", "DELETE FROM USERS WHERE UserName=\"{0}\" AND PINCode=\"{1}\"");
            Qlookup.Add("FROMRECORDIDFMT", "SELECT UserName,PINCode FROM USERS WHERE `RecordID`=\"{0}\"");
            Qlookup.Add("DROPMESSAGES", "DELETE FROM MESSAGELOG");



            //last but not least table building code...
            Qlookup.Add("DROPDBIFEXISTSFMT", @"DROP DATABASE IF EXISTS `{0}`");
            Qlookup.Add("CREATEDBFMT", @"CREATE DATABASE `{0}`; USE `{0}`");


            Qlookup.Add("CREATEORDERDATATABLE",
@"DROP TABLE IF EXISTS `OrderData`;
CREATE TABLE `OrderData` (
  `WorkID` INT(11) NOT NULL IDENTITY,
  `UserPinCode` text NOT NULL COMMENT 'PIN Code of the user for this Work Order',
  `TotalTime` bigint(20) NOT NULL COMMENT 'Total time (in ms) this user has spent on this order',
  `StartTime` datetime DEFAULT NULL COMMENT 'Starting time of current workspan, or null if user is not clocked in.',
  `Order` VARCHAR(256) NOT NULL COMMENT 'Order ID.',
  `FirstDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Set when the record is created.',
  `LastClockOut` timestamp NULL DEFAULT NULL COMMENT 'Contains the TIMESTAMP corresponding to the last time this Work Order was clocked out from.',
  PRIMARY KEY (`WorkID`)
) ;");

            Qlookup.Add("CREATEORDERSTABLE",
@"DROP TABLE IF EXISTS `TheOrders`;
CREATE TABLE `TheOrders` (
  ID int(11) NOT NULL IDENTITY,
  `OrderID` text NOT NULL,
  `Description` text NOT NULL,
PRIMARY KEY(`ID`)
);");
            Qlookup.Add("CREATEUSERSTABLE",
@"DROP TABLE IF EXISTS `USERS`;
CREATE TABLE `USERS` (
  `RecordID` int(11) NOT NULL IDENTITY,
  `UserName` varchar(256) NOT NULL,
  `PINCode` varchar(64) NOT NULL,
  PRIMARY KEY (`RecordID`)
);");
            Qlookup.Add("CREATEMESSAGELOGTABLE",
@"DROP TABLE IF EXISTS `MESSAGELOG`;
CREATE TABLE `MESSAGELOG` (
`ID` int(11) NOT NULL IDENTITY,
`LogID` varchar(64) NOT NULL DEFAULT 'GENERAL',
`Type` varchar(64) NOT NULL,
`Tstamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Set when record is created. identifies the Timestamp of this message.',
`Message` text NOT NULL COMMENT 'Message Text.',
PRIMARY KEY (`ID`)
) ;");

            Qlookup.Add("CREATECLOCKEDTABLE",
@"DROP TABLE IF EXISTS `CLOCKED`;
CREATE TABLE `CLOCKED` (
`ID` int(11) NOT NULL IDENTITY COMMENT 'ID of this record.',
`UserPIN` varchar(64) NOT NULL COMMENT 'PIN of the user clocking in or out',
`OrderID` varchar(64) NOT NULL COMMENT 'Order ID being clocked into or out of',
`STAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Timestamp of this record.',
`EventType` varchar(32) NOT NULL COMMENT 'Type of the event. usually IN or OUT to indicate clocking in or out respectively.',
PRIMARY KEY (`ID`)
);");

        }

    }



}
