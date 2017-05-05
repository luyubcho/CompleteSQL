IF OBJECT_ID('tempdb..#TestTable', 'U') IS NOT NULL
DROP TABLE #TestTable

CREATE TABLE #TestTable
(
	Number INT NOT NULL
	, Name VARCHAR(255) NOT NULL
	, Age INT NULL
	, GroupNumber INT NULL
	, GroupName VARCHAR(255) NULL
	, Salary MONEY NULL
)

Merge Into Person as tgt
Using #TestTable as src
	On tgt.Number = src.Number
When Matched
	Then Delete
When Not Matched
	Then Insert(Number, Name, Age, GroupNumber, GroupName, Salary)
	Values(src.Number, src.Name, src.Age, src.GroupNumber, src.GroupName, src.Salary);