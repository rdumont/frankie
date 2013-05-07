Feature: Watching pages
	In order to quickly visualize changes to a page
	As a site owner
	I want to have frankie watch it for changes

Background:
	Given the default directory structure
	 And the in-memory logger

Scenario: Create a new page
	Given that Frankie is watching my folder
	When I create the file 'about.html'
		"""
		<h1>About me</h1>
		File: {{ path }}
		"""
	 And wait for the watcher to finish
	Then no errors should be logged
	 And there should be an '_site/about.html' text file
		"""
		<h1>About me</h1>
		File: about.html
		"""

Scenario: Edit a page
	Given the 'about.html' text file
		"""
		<h1>About me</h1>
		"""
	 And that Frankie is watching my folder
	When I edit the file 'about.html'
		"""
		<h1>About me changed</h1>
		"""
	 And wait for the watcher to finish
	Then no errors should be logged
	And there should be an '_site/about.html' text file
        """
        <h1>About me changed</h1>
        """

Scenario: Delete a page
	Given the 'about.html' text file
        """
        <h1>About me</h1>
        """
	 And that Frankie is watching my folder
	When I delete the file 'about.html'
	 And wait for the watcher to finish
	Then no errors should be logged
	 And the file '_site/about.html' should not exist

Scenario: Rename a page
	Given the 'about.html' text file
		"""
		<h1>About me</h1>
		"""
	 And that Frankie is watching my folder
	When I rename the file 'about.html' to 'me.html'
	 And wait for the watcher to finish
	Then no errors should be logged
	 And the file '_site/about.html' should not exist
	 And there should be a '_site/me.html' text file
		"""
		<h1>About me</h1>
		"""

