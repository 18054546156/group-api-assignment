// Take-Home Assignment  
// Instruc ons:  
// • You are to keep this document and assignment details private and work on it 
// independently.  
// • If you require any clarifica ons, please email michael_labas da@simtech.a
// star.edu.sg with your ques ons.  
// • Upon comple on of the assignment (can be any me before the deadline), please 
// inform us by email to michael_labas da@simtech.a-star.edu.sg. We will then send 
// you a mee ng link to share with us your work and design thoughts.  
// Architecture: 
// Problem Statement:  
// Design and implement a RESTful API for retrieving and crea ng Group data exposed from 
// the following public API (as third-party):  
// 1. Create Group:   
// POST h ps://dev-apex
// 01.southeastasia.cloudapp.azure.com:7500/api/partner/groups 
// Input:  
// {"name": "string"} 
// 2.  Group List:   
// GET h ps://dev-apex
// 01.southeastasia.cloudapp.azure.com:7500/api/partner/groups 
// Above APIs are protected by JWT authen ca on. Get the token through authen ca on 
// using:  
// POST h ps://dev-apex-01.southeastasia.cloudapp.azure.com:7600/connect/token 
// • Input:  
// Headers 
// __tenant: T003 
// Body 
// username: applicant 
// password: 881d&793M 
// client_id: External_Integra on 
// grant_type: password 
// client_secret: 3a165ec4-6a3f-a19e-657c-0739e26cb85e 
// scope: PartnerService 
// Details:  
// • Create a private repository on GitHub for this assignment  
// • Use C#, Java or Nodejs and your preferred API frameworks (desirable C#).  
// • Implement HTTP request to create 5 new groups (API #1) prefixed with your first 
// name e.g.  
// o Michael-Group1. 
// o Michael-Group2. 
// o Michael-Group3. 
// o Michael-Group4. 
// o Michael-Group5.  
// • To validate your entries by implemen ng HTTP request to retrieve groups in API #2.  
// • State your assump ons and architectural considera ons  
// • You are free to assume everything else but make sure you document them.  
// • Bonus:  
// o Automated tests  