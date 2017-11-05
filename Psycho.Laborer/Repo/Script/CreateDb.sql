CREATE TABLE "Occupation" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_Occupation" PRIMARY KEY,
	"type" TEXT,
	"name" TEXT
);
CREATE TABLE "City" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_City" PRIMARY KEY,
	"Name" TEXT
);
CREATE TABLE "Country" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_Country" PRIMARY KEY,
	"Name" TEXT 
);
CREATE TABLE "Career" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_Career" PRIMARY KEY AUTOINCREMENT,
	"UserGetId" INTEGER NOT NULL,
	"group_id" INTEGER NOT NULL, 
	"country_id" INTEGER NOT NULL,
	"city_id" INTEGER NOT NULL,
	
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, group_id, city_id) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id),
	FOREIGN KEY(country_id) REFERENCES Country(id),
	FOREIGN KEY(city_id) REFERENCES City(id)
);
CREATE TABLE "Military" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_Military" PRIMARY KEY AUTOINCREMENT,
	"UserGetId" INTEGER NOT NULL,
	"unit" TEXT, 
	"unit_id" INTEGER,
	"Country_Id" INTEGER NOT NULL,
	
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, unit_id, Country_Id) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id),
	FOREIGN KEY(Country_Id) REFERENCES Country(id)
);

CREATE TABLE "University" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_University" PRIMARY KEY,
	"country" INTEGER, 
    "city" INTEGER,
    "name" TEXT,
    "faculty" INTEGER,
    "faculty_name" TEXT,
    "chair" INTEGER,
    "chair_name" TEXT,
    "education_form" TEXT,
    "education_status" TEXT,
    FOREIGN KEY(country) REFERENCES Country(id),
    FOREIGN KEY(city) REFERENCES City(id)
);
CREATE TABLE "School" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_School" PRIMARY KEY,
	"Country" INTEGER, 
    "City" INTEGER,
    "name" TEXT,
	"class" TEXT,
	"speciality" TEXT,
    FOREIGN KEY(Country) REFERENCES Country(id),
    FOREIGN KEY(City) REFERENCES City(id)
);

CREATE TABLE "UserUniversity" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_UserUniversity" PRIMARY KEY AUTOINCREMENT,
	
	"UserGetId" INTEGER NOT NULL,
	"UniversityId" INTEGER, 
    
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, UniversityId) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id),
	FOREIGN KEY(UniversityId) REFERENCES University(id)
);

CREATE TABLE "UserSchool" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_UserSchool" PRIMARY KEY AUTOINCREMENT,
	
	"UserGetId" INTEGER NOT NULL,
	"SchoolId" INTEGER, 
    
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, SchoolId) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id),
	FOREIGN KEY(SchoolId) REFERENCES School(id)
);


CREATE TABLE "Relative" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_Relative" PRIMARY KEY AUTOINCREMENT,
	
	"RelativeId" INTEGER NOT NULL,
	"UserGetId" INTEGER NOT NULL,
	"Type" TEXT, 
    
	CONSTRAINT SingleCareerRecord UNIQUE (Id, UserGetId) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id)
);
CREATE TABLE "FriendsFollowersSubscriptions" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_FriendsFollowersSubscriptions" PRIMARY KEY AUTOINCREMENT,
	
	"UserGetId" INTEGER NOT NULL,
	"SubjectId" INTEGER NOT NULL,
	"RelationsType" INTEGER NOT NULL,
    
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, SubjectId) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id),
	FOREIGN KEY(SubjectId) REFERENCES UserGet(id)
);
CREATE TABLE "UserGroups" (
	"Id" INTEGER NOT NULL CONSTRAINT "PK_UserGroups" PRIMARY KEY AUTOINCREMENT,
	
	"UserGetId" INTEGER NOT NULL,
	"GroupId" INTEGER NOT NULL,
	
	CONSTRAINT SingleCareerRecord UNIQUE (UserGetId, GroupId) ON CONFLICT REPLACE,
	FOREIGN KEY(UserGetId) REFERENCES UserGet(id)
);
CREATE TABLE "UserGet" (
	"id" INTEGER NOT NULL CONSTRAINT "PK_Userget" PRIMARY KEY,
    "first_name" TEXT,
    "last_name" TEXT, 
    "sex" INTEGER,  
    "nickname" TEXT, 
    "maiden_name" TEXT, 
    "domain" TEXT,  
    "screen_name" TEXT,  
    "photo_id" TEXT,  
    "has_photo" INTEGER, 
    "has_mobile" INTEGER, 
    "friend_status" INTEGER, 
    "online" INTEGER, 
    "wall_comments" INTEGER, 
    "can_post" INTEGER, 
    "can_see_all_posts" INTEGER, 
    "can_see_audio" INTEGER, 
    "can_write_private_message" INTEGER,
    "can_send_friend_request" INTEGER, 
    "site" TEXT,  
    "status" TEXT,  
    "LastSeenTime" INTEGER,
	"LastSeenPlatform" INTEGER,
    "verified" INTEGER, 
    "followers_count" INTEGER, 
    "blacklisted" INTEGER, 
    "is_favorite" INTEGER, 
    "is_hidden_from_feed" INTEGER, 
    "common_count" INTEGER,
	"OccupationId" REAL,
    "CityId" INTEGER,  
    "CountryId" INTEGER,
    "mobile_phone" TEXT,  
    "university" INTEGER, 
    "university_name" TEXT,  
    "faculty" INTEGER, 
    "faculty_name" TEXT,  
    "graduation" INTEGER, 
    "home_town" TEXT,  
    "relation" INTEGER, 
    "interests" TEXT,  
    "music" TEXT,  
    "activities" TEXT,  
    "movies" TEXT,  
    "tv" TEXT,  
    "books" TEXT,  
    "games" TEXT,  
    "about" TEXT,  
    "quotes" TEXT,  
    "home_phone" TEXT,  
    "instagram" TEXT,  
    "PersonalReligion" TEXT,
	"PersonalInspiredBy" TEXT,
	"PersonalLifeMain" INTEGER,
	"PersonalSmoking" INTEGER,
	"PersonalAlcohol" INTEGER,
	"PersonalPolitical" INTEGER,
    "bdate" TEXT, 
    "skype" TEXT,  
    "twitter" TEXT,
		
	FOREIGN KEY(OccupationId) REFERENCES Occupation(id),
	FOREIGN KEY(CountryId) REFERENCES Country(id),
	FOREIGN KEY(CityId) REFERENCES City(id)
);