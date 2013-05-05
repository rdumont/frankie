Feature: Watching posts
	In order to quickly visualize changes to a post
	As a blog author
	I want to have frankie watch it for changes

Background:
	Given the default directory structure
	 And the configuration file
		| Field     | Value                    |
		| Permalink | :year/:month/:day/:title |
	 And the in-memory logger
	 And the '_post' template
        """
        {{ post.body }}
        """

Scenario: Create a new post
	Given that Frankie is watching my folder
	When I create the file '_posts/2013-04-25-some-nice-post.md'
        """
		Cool post.
        """
	 And wait for the watcher to finish
	Then no errors should be logged
	 And there should be a '_site/2013/04/25/some-nice-post/index.html' text file
        """
        <p>Cool post.</p>
        """

Scenario: Edit a post
	Given the '_posts/2013-04-25-some-nice-post.md' text file
        """
        Cool post.
        """
	 And that Frankie is watching my folder
	When I edit the file '_posts/2013-04-25-some-nice-post.md'
		"""
		This has been edited.
		"""
	 And wait for the watcher to finish
	Then no errors should be logged
	 And there should be a '_site/2013/04/25/some-nice-post/index.html' text file
        """
        <p>This has been edited.</p>
        """

