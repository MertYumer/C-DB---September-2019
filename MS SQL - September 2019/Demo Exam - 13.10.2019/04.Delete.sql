DELETE f
  FROM Files f
  JOIN Commits c
    ON c.Id = f.CommitId
 WHERE c.RepositoryId = 3

DELETE c
  FROM Commits c
 WHERE c.RepositoryId = 3

DELETE i
  FROM Issues i
 WHERE i.RepositoryId = 3

DELETE rc
  FROM RepositoriesContributors rc
 WHERE rc.RepositoryId = 3

DELETE r
  FROM Repositories r
 WHERE r.Id = 3