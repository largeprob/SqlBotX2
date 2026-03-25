DatabaseType=[POSTGRESQL], 
DatabaseVersion=[], 
Table=[超音数数据集], 
PartitionTimeField=[数据日期 FORMAT 'yyyy-MM-dd'], 
PrimaryKeyField=[用户名], 
Metrics=[
<访问次数 COMMENT '一段时间内用户的访问次数'>,
<停留时长 COMMENT '停留时长' AGGREGATE 'SUM'>,
<人均访问次数 COMMENT '每个用户平均访问的次数'>,
<访问用户数 ALIAS 'UV,访问人数,' COMMENT '访问的用户个数'>
],
Dimensions=[
<用户名 COMMENT '用户名'>,
<用户名 COMMENT '用户名'>,
<用户名 COMMENT '用户名'>,
<页面 COMMENT '页面'>,
<部门 COMMENT '部门'>,
<数据日期 FORMAT 'yyyy-MM-dd' COMMENT '数据日期'>,
<数据日期 FORMAT 'yyyy-MM-dd' COMMENT '数据日期'>],
Values=[]

SELECT 用户名, 数据日期, SUM(访问次数) AS _访问次数_, SUM(停留时长) AS _停留时长_, COUNT(用户名) AS _UV_ FROM 超音数数据集 WHERE 数据日期 >= '2026-03-07' AND 数据日期 <= '2026-03-22' GROUP BY 用户名, 数据日期