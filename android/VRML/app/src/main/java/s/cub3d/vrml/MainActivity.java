package s.cub3d.vrml;

import android.content.DialogInterface;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.messaging.FirebaseMessaging;

public class MainActivity extends AppCompatActivity {
    private DatabaseReference mDatabase;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        FirebaseMessaging.getInstance().subscribeToTopic("state_changed");

        mDatabase = FirebaseDatabase.getInstance().getReference();

        Button submitButton = (Button) findViewById(R.id.button2);
        final Button toggleStateButton = (Button) findViewById(R.id.toggleStateButton);
        final EditText msgEditText = (EditText) findViewById(R.id.editText);

        submitButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mDatabase.child("message").setValue(msgEditText.getText().toString());
            }
        });

        toggleStateButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mDatabase.child("is_training").setValue(toggleStateButton.getText() == "Stop Training" ? 0 : 1);
            }
        });

        ValueEventListener postListener = new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                // Get Post object and use the values to update the UI
                long state = (long)dataSnapshot.getValue();
                if(state == 1) {
                    toggleStateButton.setText("Stop Training");
                } else {
                    toggleStateButton.setText("Start Training");
                }
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {
                Log.w("Tag", "loadPost:onCancelled", databaseError.toException());
            }
        };
        mDatabase.child("is_training").addValueEventListener(postListener);
    }
}