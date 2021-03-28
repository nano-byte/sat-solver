<?xml version='1.0' encoding='UTF-8' standalone='yes' ?>
<tagfile doxygen_version="1.9.1" doxygen_gitid="ef9b20ac7f8a8621fcfc299f8bd0b80422390f4b">
  <compound kind="class">
    <name>NanoByte::SatSolver::Clause</name>
    <filename>class_nano_byte_1_1_sat_solver_1_1_clause.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>Clause</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>a9069f62b8b5fdb1b3b83375bc133af77</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>Clause</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>af6f307854cb593036e3886e6a1929196</anchor>
      <arglist>(IEnumerable&lt; Literal&lt; T &gt;&gt; literals)</arglist>
    </member>
    <member kind="function">
      <type>Clause&lt; T &gt;</type>
      <name>Without</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>ad2806c095d0e04b4e0e76a2fee6c378d</anchor>
      <arglist>(Literal&lt; T &gt; literal)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Clause&lt; T &gt;</type>
      <name>operator|</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>a2bcf9234a8c2bcd018e46d66cc266d92</anchor>
      <arglist>(Clause&lt; T &gt; clause, Literal&lt; T &gt; literal)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Clause&lt; T &gt;</type>
      <name>operator|</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>a123ccb0554165a483f593bd503d1dd84</anchor>
      <arglist>(Literal&lt; T &gt; literal, Clause&lt; T &gt; clause)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Formula&lt; T &gt;</type>
      <name>operator&amp;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>ae0a9b3541eb01e477bb1c4a008acacfb</anchor>
      <arglist>(Clause&lt; T &gt; clause1, Clause&lt; T &gt; clause2)</arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsEmpty</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>a832f54de785eb97b23018aac0cba0dfe</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsUnit</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clause.html</anchorfile>
      <anchor>a7b0224280698610336d3ac3243fdac86</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::SatSolver::Clauses</name>
    <filename>class_nano_byte_1_1_sat_solver_1_1_clauses.html</filename>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; Clause&lt; T &gt; &gt;</type>
      <name>AtMostOne&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clauses.html</anchorfile>
      <anchor>ade735c4efa47869634672020d35ccd14</anchor>
      <arglist>(params Literal&lt; T &gt;[] literals)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; Clause&lt; T &gt; &gt;</type>
      <name>AtMostOne&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clauses.html</anchorfile>
      <anchor>a5356e2f130a441d23195e03027bb5162</anchor>
      <arglist>(IEnumerable&lt; Literal&lt; T &gt;&gt; literals)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; Clause&lt; T &gt; &gt;</type>
      <name>ExactlyOne&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clauses.html</anchorfile>
      <anchor>a159f7ebe176689ad57eff22ef8004420</anchor>
      <arglist>(params Literal&lt; T &gt;[] literals)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; Clause&lt; T &gt; &gt;</type>
      <name>ExactlyOne&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_clauses.html</anchorfile>
      <anchor>a1fffb6f50a5821bc521ef0ecb562b923</anchor>
      <arglist>(IEnumerable&lt; Literal&lt; T &gt;&gt; literals)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::SatSolver::Formula</name>
    <filename>class_nano_byte_1_1_sat_solver_1_1_formula.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>Formula</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>ab8a536c72d59f8a534a662078ff972fa</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>Formula</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a0fda98d1e279227b09bb08f69a13e875</anchor>
      <arglist>(IEnumerable&lt; Clause&lt; T &gt;&gt; clauses)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; Literal&lt; T &gt; &gt;</type>
      <name>GetPureLiterals</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>af42dece92c726280c78c3e3e83cae8f2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; Literal&lt; T &gt; &gt;</type>
      <name>GetLiterals</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a5218b5565995f0fe92e12a5531f775c6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>Formula&lt; T &gt;</type>
      <name>Simplify</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a0963722a47ee82069dcbe8407df8c7e1</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Formula&lt; T &gt;</type>
      <name>operator&amp;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>aa77e65c651d7df4bbaa67dd02d75c850</anchor>
      <arglist>(Formula&lt; T &gt; formula, Clause&lt; T &gt; clause)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Formula&lt; T &gt;</type>
      <name>operator&amp;</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a8ce10ed2a5efbe9a0e0bd83d2c89a791</anchor>
      <arglist>(Clause&lt; T &gt; clause, Formula&lt; T &gt; formula)</arglist>
    </member>
    <member kind="function" protection="package">
      <type>Formula&lt; T &gt;</type>
      <name>PropagateUnits</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a08ce951087a4e285b057f2e404dcadab</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="package">
      <type>Formula&lt; T &gt;</type>
      <name>EliminatePureLiterals</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a37e92e3e5588c37c2b166754103dfa51</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>ContainsEmptyClause</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>aa5d9ff08c05964dddc0560d2d3f40c18</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsConsistent</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_formula.html</anchorfile>
      <anchor>a44ac3aa496f536ecd3e3360b8c3deebe</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::SatSolver::Literal</name>
    <filename>struct_nano_byte_1_1_sat_solver_1_1_literal.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>Literal</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>ab24bb2641fa18f2249bc7e8cad705338</anchor>
      <arglist>(T value, bool negated=false)</arglist>
    </member>
    <member kind="function">
      <type>Literal&lt; T &gt;</type>
      <name>Negate</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a6c8ac5789dad0f09b5c9ca37f718f728</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>ConflictsWith</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a7d641f80a6ffc94a363b5a89778e0877</anchor>
      <arglist>(Literal&lt; T &gt; literal)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>IsPure</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a4c24c730184901e59e7580ce6736444b</anchor>
      <arglist>(IEnumerable&lt; Literal&lt; T &gt;&gt; literals)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static implicit</type>
      <name>operator Literal&lt; T &gt;</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>ad09a3368dff8aa95ef6a13c5878881b9</anchor>
      <arglist>(T value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Literal&lt; T &gt;</type>
      <name>operator!</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a355046595f2660b33f3b9967fd32f7d7</anchor>
      <arglist>(Literal&lt; T &gt; literal)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static implicit</type>
      <name>operator Clause&lt; T &gt;</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a6e575486115228e4b6646cc6d732a10c</anchor>
      <arglist>(Literal&lt; T &gt; literal)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Clause&lt; T &gt;</type>
      <name>operator|</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>ad93bd10d297d0f51eb5f4749a90a60b0</anchor>
      <arglist>(Literal&lt; T &gt; literal1, Literal&lt; T &gt; literal2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Formula&lt; T &gt;</type>
      <name>operator&amp;</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>ae4ae264932ed3afdad3972d1b4e0c7f3</anchor>
      <arglist>(Literal&lt; T &gt; literal1, Literal&lt; T &gt; literal2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Literal&lt; T &gt;</type>
      <name>Of&lt; T &gt;</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a5b73f6315b344a62d377349d8efcb957</anchor>
      <arglist>(T value)</arglist>
    </member>
    <member kind="property">
      <type>T</type>
      <name>Value</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>a00ec0474baf6de4eae0542a4cd0bf9b5</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>Negated</name>
      <anchorfile>struct_nano_byte_1_1_sat_solver_1_1_literal.html</anchorfile>
      <anchor>ac4e822175235d9d42ac0a834a2cd3f9f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::SatSolver::Solver</name>
    <filename>class_nano_byte_1_1_sat_solver_1_1_solver.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>bool</type>
      <name>IsSatisfiable</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_solver.html</anchorfile>
      <anchor>a98eb2f67885fdd424b78fd5eb15d45e9</anchor>
      <arglist>(Formula&lt; T &gt; formula)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual Literal&lt; T &gt;</type>
      <name>ChooseLiteral</name>
      <anchorfile>class_nano_byte_1_1_sat_solver_1_1_solver.html</anchorfile>
      <anchor>abc5313d02d9b25e4d5de8399c6f4a4dd</anchor>
      <arglist>(Formula&lt; T &gt; formula)</arglist>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::SatSolver</name>
    <filename>namespace_nano_byte_1_1_sat_solver.html</filename>
    <class kind="class">NanoByte::SatSolver::Clause</class>
    <class kind="class">NanoByte::SatSolver::Clauses</class>
    <class kind="class">NanoByte::SatSolver::Formula</class>
    <class kind="class">NanoByte::SatSolver::Literal</class>
    <class kind="class">NanoByte::SatSolver::Solver</class>
  </compound>
  <compound kind="page">
    <name>index</name>
    <title></title>
    <filename>index.html</filename>
    <docanchor file="index.html">md_D__a_sat_solver_sat_solver_doc_main</docanchor>
  </compound>
</tagfile>
