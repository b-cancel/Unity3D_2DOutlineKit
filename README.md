# Unity3D_2DOutlineKit_INPROG

<h5>If this project helped you reduce developement time or you just want to help me continue making useful tools</h5>
<h5>Feel free to buy me a cup of coffee! :)</h5>
<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=bryan%2eo%2ecancel%40gmail%2ecom&lc=US&item_name=Cup%20Of%20Coffee&item_number=0000&no_note=0&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHostedGuest">
  <img src="https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif" alt="PayPal Donate Button">
</a>
<h4>How To Use</h4>
<ul>
  <li>for a convex sprite, drag the "convexOut" script into the gameobject or add it as a component</li>
  <li>for a concave sprite, drag the "concaveOut" script into the gameobject or add it as a component</li>
</ul>
<h4>All Test Basic Tests and Features</h4>
<ul>
  <li>
    Basics
    <ul>
      <li>viewable in scene view</li>
      <li>viewable in game view</li>
      <li>viewable in edit mode</li>
      <li>viewable in game mode</li>
    </ul>
  </li>
  <li>
    Camera
    <ul>
      <li>both perspective and orthographic mode</li>
      <li>all rendering paths</li>
      <li>occlusion culling (on or off)</li>
      <li>allow hdr (on or off)</li>
      <li>allow msaa (on or off)</li>
      <li>allow dynamic resolution (on or off)</li>
    </ul>
  </li>
  <li>
    Follows Object Transform
    <ul>
      <li>position</li> 
      <li>rotation</li> 
      <li>scale</li>
    </ul>
  </li>
  <li>
    Inspector Options
    <ul>
      <li>remove component</li>
    </ul>
  </li>
  <li>
    Hierarchy Right Click Options
    <ul>
      <li>copy</li>
      <li>paste</li>
      <li>duplicate</li>
      <li>delete</li>
    </ul>
  </li>
  <li>
    Sprite Renderer
    <ul>
      <li>can switch sprite</li>
      <li>can change sprite color with no effect on outline or overlay</li>
      <li>flip x and flip y</li>
      <li>draw mode (simple)</li>
    </ul>
  </li>
  <li>
    Extras
    <ul>
      <li>creating a prefab and making multiple copies of that prefab</li>
      <li>multiple outlines of different types on the same object (for rainbow color outlines)</li>
      <li>works with animated sprites (by setting the "Animator" "Update Mode" to "Animate Physics”)</li>
    </ul>
  </li>
</ul>
<h4>Limitations</h4>
<ul>
  <li>Since I am using the sprite itself to create it's outline, if the sprite is semi transparent, then the outline and the overlay will also be semi transparent</li>
  <li>"Draw Mode Sliced" Does Not Work if you use a clipping mask (because Unity's Sprite Mask doesn't currently work with its 9 slice system)</li>
  <li>"Draw Mode Tiled" Does Not Work (because Unity's Sprite Mask doesn't currently work with it)</li>
  <li>The Standard "Reset" Option Does Not Work (use the built in reset button instead)</li>
  <li>The "Copy Component" Option Does Not Work and therefore the "Paste Component as New" and "Paste Component Value" Options Dont Work Either</li>
</ul>
<h4>Outline Features</h4>
<table style="width:100%; border: 1px solid black; text-align:center;">
  <tr>
    <td><b>CONCAVE</b><br>(for concave sprites)</td>
    <td><b>CONVEX</b><br>(for convex sprites)</td>
  </tr>
  <tr>
    <th>OPTIMIZATION VARIABLES</th>
  </tr>
  <tr>
    <td>Update Sprite Every Frame</td>
  </tr>
  <tr>
    <th>DEBUGGING VARIABLES</th>
  </tr>
  <tr>
    <td>Show Outline Game Objects in Hierarchy</td>
  </tr>
  <tr>
    <td>SPRITE OVERLAY VARIABLES</td>
  </tr>
  <tr>
    <td>Active</td>
  </tr>
  <tr>
    <td>Order in Layer</td>
  </tr>
  <tr>
    <td>Color</td>
  </tr>
  <tr>
    <th>CLIPPING MASK VARIABLES<br>(allows outlines to work well with semi transparent sprites)</th>
  </tr>
  <tr>
    <td>Clip Center</td>
  </tr>
  <tr>
    <td>Alpha Cut Off<br>(only relevant if “Clip Center” == True)</td>
  </tr>
  <tr>
    <td>Custom Range<br>(only relevant if “Clip Center” == True)</td>
  </tr>
  <tr>
    <td>Front Layer<br>(only relevant if “Custom Range” == True)</td>
  </tr>
  <tr>
    <td>Back Layer<br>(only relevant if “Custom Range” == True)</td>
  </tr>
  <tr>
    <th>OUTLINE VARIABLES</th>
  </tr>
  <tr>
    <td>Active</td>
  </tr>
  <tr>
    <td>Color</td>
  </tr>
  <tr>
    <td>Order in Layer</td>
  </tr>
  <tr>
    <td>Size (world space)</td>
    <td>Size (local scale)</td>
  </tr>
  <tr>
    <td>Outline Scale X With Sprite</td>
  </tr>
  <tr>
    <td>Outline Scale Y With Sprite</td>
  </tr>
  <tr>
    <td>Force Retain Proportions With Children</td>
  </tr>
  <tr>
    <th>PUSH VARIABLES</th>
  </tr>
  <tr>
    <td>Push Type is Regular or Custom</td>
    <td></td>
  </tr>
  <tr>
    <td>Count of Objects Making Outline</td>
    <td></td>
  </tr>
  <tr>
    <td>Start Angle<br>(for type regular)</td>
    <td></td>
  </tr>
  <tr>
    <td>Push Type Radial or Square<br>(for type regular)</td>
    <td></td>
  </tr>
  <tr>
    <td>Std Size<br>(for type custom)</td>
    <td></td>
  </tr>
</table>
<br>
<h4><a href="https://docs.google.com/document/d/1UCxu07cAwVSxPS3i7ouwJIBSrV2yh9v0bNlYWYeJJcs/edit?usp=sharing">
Documentation
</a></h4>
<h4><a href="https://docs.google.com/document/d/1wpzp4dFecQ3u8pj6IuYlhem_of8CiI_OEGuR32aKG_w/edit?usp=sharing">
Comparison To Other Assets
</a></h4>
<h5>
<h4>Shader Help Desired</h4>
<h5>(because I have searched but everything I have found requires too many edits [that I dont know how to make] or doesn't work)</h5>
<ul>
  <li>creation or location of a shader that can color any sprite a solid color, without copying the alpha like the one I am using does</li>
  <li>creation of location of a shader with all these <a href="https://docs.google.com/document/d/1ASiDM8Ra5F9e-VTWzHEJcNvDyBeABuzv6KxhkgsPKtM/edit?usp=sharing">specifications</a>. This should make the asset faster and remove many limitations.</li>
</ul>
<h4>Note: If you need parent and child relationships I might be willing to help, email me. It isn't hard, just situation specific.</h4>
